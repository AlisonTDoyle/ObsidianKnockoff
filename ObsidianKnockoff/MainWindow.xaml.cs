using ObsidianKnockoff.Classes;
using ObsidianKnockoff.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ObsidianKnockoff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // properties
        private const string PENDING_SAVE_TEXT = "Pending Save...";
        private const string SAVING_TEXT = "Saving...";
        private const string SAVED_TEXT = "Saved!";
        private const int SAVE_PERIOD_IN_SECONDS = 10;

        private FileHandlerService _fileHandlerService;
        private BackgroundWorker _fileViewerBackgroundWorker = new BackgroundWorker();
        private BackgroundWorker _fileLoaderBackgroundWorker = new BackgroundWorker();
        private DispatcherTimer _fileViewerTimer = new DispatcherTimer();
        private Timer _saveFileThreadTimer;

        public ObservableCollection<string> FileNames = new ObservableCollection<string>();

        // constructors
        public MainWindow()
        {
            InitializeComponent();

            // set up ui
            lblSavingStatus.Content = PENDING_SAVE_TEXT;

            // set up services
            _fileHandlerService = new FileHandlerService();

            // set up background worker and timer
            _fileViewerBackgroundWorker.DoWork += FileViewerBackgroundWorker_DoWork;
            _fileViewerBackgroundWorker.ProgressChanged += FileViewerBackgroundWorker_ProgressChanged;
            _fileViewerBackgroundWorker.RunWorkerCompleted += FileViewerBackgroundWorker_RunWorkerCompleted;
            _fileViewerBackgroundWorker.WorkerReportsProgress = true;

            _fileViewerTimer.Interval = TimeSpan.FromSeconds(5);
            _fileViewerTimer.Tick += RefreshTimer_Tick;
            _fileViewerTimer.Start();

            // set up saving file timer thread
            _saveFileThreadTimer = new Timer(
                callback: _ => SaveFile(),
                state: null,
                dueTime: TimeSpan.FromSeconds(SAVE_PERIOD_IN_SECONDS),          
                period: TimeSpan.FromSeconds(SAVE_PERIOD_IN_SECONDS)
            );
        }

        // event handlers
        protected override void OnClosed(EventArgs e)
        {
            // kill timer when app closes
            _fileViewerTimer.Stop();
            base.OnClosed(e);
        }

        private void FileViewerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // store result in e.Result to safely pass back to UI thread
            e.Result = _fileHandlerService.ReadFilesInFolder(worker);
        }

        private void FileViewerBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                prgSearchProgress.Value = e.ProgressPercentage;
            });
        }

        private void FileViewerBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error");
                return;
            }

            List<string> fileNames = e.Result as List<string>;

            this.Dispatcher.Invoke(() =>
            {
                // update ui with the file list here
                lbxFiles.ItemsSource = fileNames;
                prgSearchProgress.Value = 0;
            });
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // avoid starting a new run if one is already in progress
            if (!_fileViewerBackgroundWorker.IsBusy)
            {
                _fileViewerBackgroundWorker.RunWorkerAsync();
            }
        }

        // methods
        private void SaveFile()
        {
            string title = "";
            string content = "";

            Dispatcher.Invoke(() =>
            {
                title = tbxFileName.Text;
                content = tbxFileContent.Text;
            });

            Console.WriteLine($"Title: {title}, Content: {content}");

            // check if user has entered any details (selected file or creating new one)
            if (!string.IsNullOrEmpty(title))
            {
                // let user know file is being saved
                Dispatcher.Invoke(() =>
                {
                    lblSavingStatus.Content = SAVING_TEXT;
                });

                Thread.Sleep(1000);

                Note newNote = new Note(title, content);
                _fileHandlerService.SaveFile(newNote);

                // let user know file has saved
                Dispatcher.Invoke(() =>
                {
                    lblSavingStatus.Content = SAVED_TEXT;
                });

                Thread.Sleep(1000);

                // return text to pending state
                Dispatcher.Invoke(() =>
                {
                    lblSavingStatus.Content = PENDING_SAVE_TEXT;
                });
            }
        }
    }
}
