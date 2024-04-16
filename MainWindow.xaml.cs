using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Input;

namespace MarkdownWriter
{
    public static class FileNameReader
    {
        public static string[] GetFileNames(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                return Directory.EnumerateFiles(directoryPath)
                    .Select(Path.GetFileName)
                    .ToArray();
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }
        }
    }
    
    public partial class MainWindow : Window
    {
        private static string directloc = "C:\\Users\\mhlev\\Documents\\Obsidian\\ObsidianVault\\EnglishDB\\";
        string[] files = FileNameReader.GetFileNames(directloc);
            public class Option
             {
                 public string Name { get; set; }
             }
        public ObservableCollection<Option> Options { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Options = new ObservableCollection<Option> { };
            foreach (string file in files)
            {
                Options.Add(new Option() { Name = file });
            }
            DataContext = this;;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            DragMove();
        }
        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            string path = txtFilePath.Text;
            string filePath = "C:\\Users\\mhlev\\Documents\\Obsidian\\ObsidianVault\\EnglishDB\\"+path;
            string contentToWrite = string.Format("[{0}](https://dictionary.cambridge.org/dictionary/english/{0})- ",txtContent.Text);

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please enter a file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(contentToWrite);
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