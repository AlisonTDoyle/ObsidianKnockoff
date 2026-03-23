using ObsidianKnockoff.Classes;
using ObsidianKnockoff.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObsidianKnockoff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // properties
        private string _currentDirectory = Directory.GetCurrentDirectory();

        private FileHandlerService _fileHandlerService;
        private Thread _fileMonitoringThread;
        private Thread _queryHandlingThread;

        private ObservableCollection<string> _files = new ObservableCollection<string>();
        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        
        public ObservableCollection<string> Files { get { return _files; } }

        // constructors
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            string noteVaultPath = _currentDirectory + "\\NotesFolder";
            Console.WriteLine(noteVaultPath);
            _fileHandlerService = new FileHandlerService(noteVaultPath);
        }

        // event handlers
        private void btnAddNewFile_Click(object sender, RoutedEventArgs e)
        {
        }

        // methods
    }
}
