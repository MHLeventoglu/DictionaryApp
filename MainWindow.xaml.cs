using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace PersonalDictionary
{
    public partial class MainWindow
    {
        private static string directloc = "C:\\";
        private static string programFolderName = "DictionaryApp";
        private string configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            programFolderName,
            "config.json"
        );
        string[] files = FileNameReader.GetFileNames(directloc);

        public class Option
        {
            public string Name { get; set; }
        }

        public ObservableCollection<Option> Options { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Options = new ObservableCollection<Option>();

            LoadConfig();
            
            files = FileNameReader.GetFileNames(directloc);
            foreach (string file in files)
            {
                Options.Add(new Option() { Name = file });
            }

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            DragMove();
        }


        private OpenFolderDialog Select_Directory()
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog
            {
                Title = "Select Folder",
                InitialDirectory = directloc
            };

            folderDialog.ShowDialog();

            return folderDialog;
        }

        private void CreateFolderIfNotExists()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), programFolderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private void btnFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = Select_Directory();
            // Check if user selected a folder
            if (!string.IsNullOrEmpty(folderDialog.FolderName))
            {
                directloc = folderDialog.FolderName;
                files = FileNameReader.GetFileNames(directloc);

                Options.Clear();
                foreach (string file in files)
                {
                    Options.Add(new Option() { Name = file });
                }

                RaisePropertyChanged("Options");

                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            try
            {
                CreateFolderIfNotExists();

                var config = new { LastDirectory = directloc };
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(config, options);

                File.WriteAllText(configFilePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred saving configuration: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string jsonString = File.ReadAllText(configFilePath);
                    var config = JsonSerializer.Deserialize<dynamic>(jsonString);
                    directloc = config?.GetProperty("LastDirectory").GetString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred loading configuration: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            string path = txtFilePath.Text;
            string filePath = Path.Combine(directloc, path);
            string contentToWrite = string.Format("[{0}](https://dictionary.cambridge.org/dictionary/english/{0})- ", txtContent.Text);

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please enter a file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine("\t" + contentToWrite);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            txtContent.Text = string.Empty;
        }
    }
}
