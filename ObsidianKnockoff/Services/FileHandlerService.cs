using ObsidianKnockoff.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

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
        public void SaveFile(Note note)
        {
            string fileName = note.GetFileName();
            string filePath = $"{NOTES_FOLDER}/{fileName}";

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

                        // create/write to file
                        using (IsolatedStorageFileStream stream = _isolatedStorageFile.OpenFile(filePath, FileMode.Create))
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                writer.Write(note.Content);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error");
                    }
                }
            }
        }

        public Note ReadNote(string noteName)
        {
            Note note = new Note();
            string fileContent = "";
            string filePath = $"{NOTES_FOLDER}/{noteName}";

            if (_isolatedStorageFile != null)
            {
                lock (fileLock)
                {
                    try
                    {
                        if (_isolatedStorageFile.DirectoryExists(NOTES_FOLDER))
                        {
                            string searchPattern = $"{NOTES_FOLDER}/*.txt";
                            using (IsolatedStorageFileStream stream = _isolatedStorageFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                            {
                                using (StreamReader reader = new StreamReader(stream))
                                {
                                    fileContent = reader.ReadToEnd();
                                }
                            }            
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error");
                    }
                }
            }

            note = new Note(fileContent);
            note.SetNoteTitleFromFileName(noteName);

            return note;
        }

        public List<string> ReadFilesInFolder(BackgroundWorker worker)
        {
            List<string> fileNames = new List<string>();

            if (_isolatedStorageFile != null)
            {
                lock (fileLock)
                {
                    try
                    {
                        if (_isolatedStorageFile.DirectoryExists(NOTES_FOLDER))
                        {
                            string searchPattern = $"{NOTES_FOLDER}/*.txt";
                            string[] files = _isolatedStorageFile.GetFileNames(searchPattern);
                            int totalFiles = files.Length;

                            for (int i = 0; i < totalFiles; i++)
                            {
                                string fileName = files[i];
                                fileNames.Add(fileName);

                                // report progress back to background worker
                                double progress = ((float)(i + 1) / totalFiles) * 100;
                                worker.ReportProgress((int)Math.Round(progress));

                                // sleep for half a second to show progress
                                Thread.Sleep(500);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error");
                    }
                }
            }

            return fileNames;
        }
    }
}
