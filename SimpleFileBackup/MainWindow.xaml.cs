// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleFileBackup
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private readonly string _versionFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" +
                                                   "SimpleFileBackupVersionHistory.txt";
        private StorageFolder _rootPath;
        public List<string> DeletedFiles = new List<string>();
        public List<string> ModifiedFiles = new List<string>();
        public List<string> AddedFiles = new List<string>();

        private async void RootButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var windowId = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowId);
            _rootPath = await folderPicker.PickSingleFolderAsync();
            if (_rootPath != null)
            {
                RootButton.Content = _rootPath.DisplayName;
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForFirstScan())
            {
                CreateVersionFile();
                WriteInitialVersionFile();
            }
            else
            {
                CollectChanges();
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<string> GetAllFiles()
        {
            string path = _rootPath.Path;
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList();
        }

        private bool CheckForFirstScan()
        {
            if (!File.Exists(_versionFilePath))
                return true;
            return false;
        }

        private void WriteInitialVersionFile()
        {
            File.WriteAllLines(_versionFilePath, CollectFiles());
        }

        private List<string> CollectFiles()
        {
            var rawAllFiles = GetAllFiles();
            var allFiles = new List<string>();
            foreach (var file in rawAllFiles)
            {
                FileInfo fi = new FileInfo(file);
                var modifiedDate = fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString();
                allFiles.Add(file + " || " + modifiedDate);
            }
            return allFiles;
        }

        private void CollectChanges()
        {
            var originalList = File.ReadAllLines(_versionFilePath);
            var currentList = CollectFiles();
            var differences = new List<string>();
            IEnumerable<string> diff = originalList.Except(currentList);
            foreach (string s in diff)
            {
                differences.Add(s);
            }

            var diffNames = new List<string>();
            foreach (var file in differences)
            {
                diffNames.Add(GetFileNameFromList(file));
            }

            foreach (var file in diffNames)
            {
                if (originalList.Contains(file) && currentList.Contains(file))
                {
                    ModifiedFiles.Add(file);
                }
                if (originalList.Contains(file) && !currentList.Contains(file))
                {
                    DeletedFiles.Add(file);
                }
                if (!originalList.Contains(file) && currentList.Contains(file))
                {
                    AddedFiles.Add(file);
                }

            }
        }

        private string GetFileNameFromList(string s)
        {
            var t = s.Split(" || ");
            return t.First();
        }

        private void CreateVersionFile()
        {
            File.Create(_versionFilePath).Close();
        }
    }
}
