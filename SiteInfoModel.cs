using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiFTPUploader
{
    public class SiteInfoModel
    {
        public string directoryPath { get; set; }
        public string PathWinSCP { get; set; }
        public List<string> FilesToExclude { get; set; }
        public List<string> FoldersToExclude { get; set; }
        public List<Site> Sites { get; set; }
        public int filesToTake { get; set; }       
    }

    public class Site
    {
        public string ftp_url { get; set; }
        public string root_folder { get; set; }
    }
}
