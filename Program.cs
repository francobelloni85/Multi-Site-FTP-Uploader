using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace MultiFTPUploader
{
    internal class Program
    {
        public static string line = "--------------------------------------------------------------";

        static void Main(string[] args)
        {

            try
            {
                string jsonText = File.ReadAllText("websites.json");
                var websiteInfo = JsonConvert.DeserializeObject<SiteInfoModel>(jsonText);

                List<FileInfo> files = GetFileList(websiteInfo.directoryPath, websiteInfo.filesToTake, websiteInfo.FoldersToExclude, websiteInfo.FilesToExclude);

                Console.WriteLine($"Top {websiteInfo.filesToTake} recently modified files:");
                Console.WriteLine("+-----+------------------------------------------------------------------------------------------------------+---------------------+");
                Console.WriteLine("| No. | File Path                                                                                            | Last Modified       |");
                Console.WriteLine("+-----+------------------------------------------------------------------------------------------------------+---------------------+");

                int counter = 1;
                foreach (var file in files)
                {
                    Console.WriteLine($"| {counter.ToString().PadRight(3)} | {file.FullName.PadRight(100).Substring(0, 100)} | {file.LastWriteTime} |");
                    counter++;
                }
                Console.WriteLine("+-----+------------------------------------------------------------------------------------------------------+---------------------+");

                int countFilesToTake = GetFilesToTake("How many files do you want to transfer? ",1, files.Count);
                int countFilesSkip = GetFilesToTake("File to skip? ", 0, files.Count);

                var filesToUpdate = files.Skip(countFilesSkip).Take(countFilesToTake);

                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploadScript.txt");
                Console.WriteLine("----");

                foreach (var site in websiteInfo.Sites)
                {
                    using (StreamWriter writer = new StreamWriter(scriptPath, false))
                    {
                        writer.WriteLine($"open {site.ftp_url}");
                        foreach (var file in filesToUpdate)
                        {
                            // Ottieni il percorso relativo del file rispetto alla directory principale
                            string relativePath = file.FullName.Substring(websiteInfo.directoryPath.Length).TrimStart('\\');
                            // Converti i backslash in slash e costruisci il percorso del server
                            string serverPath = $"/{site.root_folder}/{relativePath.Replace("\\", "/")}";
                            string command = $"put {file.FullName} {serverPath}";
                            writer.WriteLine(command);
                        }
                        writer.WriteLine("exit");
                    }

                    if (ExecuteScript(websiteInfo.PathWinSCP, scriptPath))
                    {
                        Console.WriteLine(line);
                        Console.WriteLine($"{site.root_folder} - Upload done");
                        Console.WriteLine(line);
                        Console.WriteLine(" ");
                    }
                    else
                    {
                        Console.WriteLine(line);
                        Console.WriteLine($"{site.root_folder} - fail");
                        Console.WriteLine(line);
                        Console.WriteLine(" ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static bool ExecuteScript(string winscpPath, string scriptPath)
        {

            // Crea un nuovo processo
            Process process = new Process();
            process.StartInfo.FileName = winscpPath;
            process.StartInfo.Arguments = $"/script={scriptPath}";

            // Reindirizza l'output standard
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Avvia il processo e cattura l'output
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Scrivi l'output in un file o analizzalo come necessario
            File.WriteAllText("output.txt", output);

            Console.Write(output);

            // Controlla il valore di uscita
            if (process.ExitCode == 0)
            {
                return true;
            }
            return false;

        }


        private static List<FileInfo> GetFileList(string path, int countFile = 20, List<string>? excludedFolders = null, List<string>? excludedFiles = null)
        {
            var allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                    .Select(file => new FileInfo(file));

            // Escludi i file nelle cartelle escluse
            if (excludedFolders != null)
            {
                foreach (var excludedFolder in excludedFolders)
                {
                    string excludedFolderPath = Path.Combine(path, excludedFolder.TrimStart('\\'));
                    allFiles = allFiles.Where(file => !file.FullName.StartsWith(excludedFolderPath, StringComparison.OrdinalIgnoreCase));
                }
            }

            // Escludi i file specifici
            if (excludedFiles != null)
            {
                allFiles = GetListWithFilterByFileName(path, allFiles, excludedFiles);
            }

            return allFiles.OrderByDescending(file => file.LastWriteTime)
                           .Take(countFile)
                           .ToList();
        }


        private static List<FileInfo> GetListWithFilterByFileName(string path, IEnumerable<FileInfo> allFiles, List<string> excludedFiles)
        {

            List<FileInfo> filteredFiles = new List<FileInfo>();

            foreach (var file in allFiles)
            {
                bool isExcluded = false;
                foreach (var excludedFile in excludedFiles)
                {
                    if (file.Name.Equals(excludedFile, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }
                }
                if (!isExcluded)
                {
                    filteredFiles.Add(file);
                }
            }

            return filteredFiles;

        }

        private static int GetFilesToTake(string message, int minFileToTake, int maxFileToTake)
        {
            Console.Write(message);
            int numberOfFiles;
            while (true)
            {
                string? input = Console.ReadLine();
                if (int.TryParse(input, out numberOfFiles) && numberOfFiles >= minFileToTake && numberOfFiles < maxFileToTake)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter a valid number greater than 0 and less than " + maxFileToTake);
                }
            }
            return numberOfFiles;
        }

    }
}