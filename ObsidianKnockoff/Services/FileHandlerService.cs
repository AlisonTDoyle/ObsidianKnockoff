using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianKnockoff.Services
{
    internal class FileHandlerService
    {
        // properties
        private const string NOTES_FILE_TYPE = "*.txt";

        private string _notesFolderPath = "";

        // constructors
        public FileHandlerService() { }

        public FileHandlerService(string path)
        {
            _notesFolderPath = path;
        }

        // methods
        public List<string> ReadFilesInFolder()
        {
            List<string> fileNames = new List<string>();

            // get files from folder
            DirectoryInfo notesFolder = new DirectoryInfo(_notesFolderPath);
            List<FileInfo> files = notesFolder.GetFiles(NOTES_FILE_TYPE).ToList();

            // parse out necessary information
            foreach (FileInfo file in files)
            {
                string fileName = (file.Name).Replace(".txt", "");
                fileNames.Add(fileName);
            }

            return fileNames;
        }
    }
}
