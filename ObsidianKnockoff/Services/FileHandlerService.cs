using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using ObsidianKnockoff.Classes;

namespace ObsidianKnockoff.Services
{
    internal class FileHandlerService
    {
        // properties
        private const string NOTES_FOLDER = "notes";

        private static Object fileLock = new Object();

        private IsolatedStorageFile _isolatedStorageFile;

        // constructors
        public FileHandlerService() 
        {
            _isolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain();
        }

        // methods
        public void CreateNewFile(Note newNote)
        {
            string fileName = GetFileName(newNote);

            // check if isolated storage was obtained
            if (_isolatedStorageFile != null)
            {
                // sync access to file
                lock (fileLock)
                {
                    try
                    {
                        // check if notes folder was created
                        if (!_isolatedStorageFile.DirectoryExists(NOTES_FOLDER))
                        {
                            // create folder if it doesn't already exist
                            _isolatedStorageFile.CreateDirectory(NOTES_FOLDER);
                        }

                        // create storage file

                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
        }

        public void ReadInAllFiles()
        {

        }

        public void UpdateFileInformation()
        {

        }

        private string GetFileName(Note note)
        {
            string fileName = note.Title.Replace(" ", "_");
            fileName = $"{fileName}.txt";

            return fileName;
        }
    }
}
