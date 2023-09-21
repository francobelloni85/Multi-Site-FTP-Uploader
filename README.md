# Multi-Site FTP Uploader

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Multi-Site FTP Uploader is a robust .NET-based application designed to facilitate the automated uploading of files to multiple FTP sites simultaneously. It leverages a user-friendly JSON configuration file, allowing users to easily specify various FTP sites and designate files and folders to be excluded during the upload process.

## 🌟 Features

- **Bulk Uploads:** Efficiently upload files to multiple FTP sites with a single command.
- **Exclusion Lists:** Conveniently exclude specific files and folders from the upload.
- **Customizable Settings:** Easily configure the number of files to be uploaded and designate different root folders for different FTP sites.
- **Detailed Logging:** Keep track of the upload process through comprehensive logs.

## 🛠 Prerequisites

- [.NET Core](https://dotnet.microsoft.com/download/dotnet-core) or [.NET 5+](https://dotnet.microsoft.com/download/dotnet/5.0)
- [WinSCP](https://winscp.net/eng/download.php)

## 🚀 Installation & Setup

1. **Clone the Repository**
```sh
git clone https://github.com/francobelloni85/Multi-Site-FTP-Uploader.git
```

2. **Configuration**

Modify the websites.json file in the project directory to configure the FTP sites, files, and folders.

### Example Configuration (`websites.json`):

```json
{
    "directoryPath": "C:\\path\\to\\your\\files",
    "winscpPath": "C:\\path\\to\\WinSCP.com",
    "filesToExclude": ["file1.txt", "file2.txt"],
    "foldersToExclude": ["folder1", "folder2"],
    "filesToTake": 10,
    "Sites": [
        {
            "ftp_url": "ftp.site1.com",
            "root_folder": "site1"
        },
        {
            "ftp_url": "ftp.site2.com",
            "root_folder": "site2"
        }
    ]
}
```

## 🚀 Usage

1. ### Configure the JSON File
After modifying the websites.json file with your preferences and FTP details, save it.

2. ### Run the Application
Navigate to the project directory in your terminal and run: dotnet run

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check issues page.

## 📜 License

This project is MIT licensed.