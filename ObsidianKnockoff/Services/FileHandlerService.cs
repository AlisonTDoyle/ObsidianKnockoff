using ObsidianKnockoff.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ObsidianKnockoff.Services
{
    internal class FileHandlerService
    {
        // properties
        private const string NOTES_FOLDER = "notes";

        private static Object _fileLock = new Object();
        private static bool _isSaving = false;

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
                Monitor.Enter(_fileLock);
                try
                {
                    // signal that a save is in progress
                    _isSaving = true;

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
                finally
                {
                    // signal save complete
                    _isSaving = false;
                    Monitor.PulseAll(_fileLock);
                    Monitor.Exit(_fileLock);
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
                lock (_fileLock)
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
                Monitor.Enter(_fileLock);
                try
                {
                    // if a save is currently in progress, wait until it signals completion
                    while (_isSaving)
                    {
                        Monitor.Wait(_fileLock);
                    }

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

                            // sleep for half a second to show progress bar
                            Thread.Sleep(500);
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error");
                }
                finally
                {
                    Monitor.Exit(_fileLock);
                }
            }

            return fileNames;
        }

        public List<Note> SearchNotes(string query)
        {
            List<Note> matchingNotes = new List<Note>();
            string[] keywords = query.ToLower().Split(' ');

            Monitor.Enter(_fileLock);
            try
            {
                while (_isSaving) Monitor.Wait(_fileLock);

                if (_isolatedStorageFile.DirectoryExists(NOTES_FOLDER))
                {
                    string[] files = _isolatedStorageFile.GetFileNames($"{NOTES_FOLDER}/*.txt");

                    foreach (string fileName in files)
                    {
                        Note note = ReadNote(fileName);
                        string noteText = note.Content.ToLower();

                        // include note if any keyword matches
                        bool isRelevant = keywords.Any(k => noteText.Contains(k));
                        if (isRelevant)
                            matchingNotes.Add(note);
                    }
                }
            }
            finally
            {
                Monitor.Exit(_fileLock);
            }

            return matchingNotes;
        }
    }
}
