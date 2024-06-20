using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Components
{
    public class FileSystemEntryDTO
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Type { get; set; }
        public string? Size { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? Created { get; set; }
        public string? FolderPath { get; set; }
    }

    public class FileWithFolder
    {
        public string File { get; }
        public string FolderPath { get; }

        public FileWithFolder(string filePath, string folderPath)
        {
            File = filePath;
            FolderPath = folderPath;
        }
    }
}
