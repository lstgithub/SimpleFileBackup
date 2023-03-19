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

        // Most important file to ensure that only modified/added/deleted files will be taken to work
        public readonly string VersionFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" +
                                                   "SimpleFileBackupVersionHistory.txt";
        public StorageFolder SourcePath; // Source location is ALWAYS used as a latest and most up-to-date file system state. After click on "Start", destination location will be mirrored
        public StorageFolder DestinationPath;
        public List<string> DeletedFiles = new();
        public List<string> ModifiedFiles = new();
        public List<string> AddedFiles = new();

        private async void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var windowId = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowId);
            SourcePath = await folderPicker.PickSingleFolderAsync();
            if (SourcePath != null)
            {
                SourceButton.Content = SourcePath.DisplayName;
            }
        }

        private async void DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var windowId = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, windowId);
            DestinationPath = await folderPicker.PickSingleFolderAsync();
            if (DestinationPath != null)
            {
                DestinationButton.Content = DestinationPath.DisplayName;
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear lists to reflect the latest file system changes on each click
            DeletedFiles.Clear();
            AddedFiles.Clear();
            ModifiedFiles.Clear();

            // Ensure is it an initial launch of the app or not
            if (CheckForFirstScan())
            {
                CreateVersionFile();
                WriteInitialVersionFile();
                CollectChanges();
            }
            else
            {
                CollectChanges();
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //StartFilesHandling();
        }

        private void StartFilesHandling()
        {
            DeleteFiles(DeletedFiles);
            UpdateFiles(ModifiedFiles);
            AddFiles(AddedFiles);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<string> GetAllFiles(StorageFolder folder)
        {
            return Directory.GetFiles(folder.Path, "*", SearchOption.AllDirectories).ToList();
        }

        private bool CheckForFirstScan()
        {
            // Check for file presence and non-zero size. If not true for any statement, file will be re-created
            if (File.Exists(VersionFilePath))
            {
                try
                {
                    if (new FileInfo(VersionFilePath).Length > 0)
                        return false;
                }
                catch { }
            }
            return true;
        }

        private void WriteInitialVersionFile()
        {
            File.WriteAllLines(VersionFilePath, CollectFiles(SourcePath));
        }

        private List<string> CollectFiles(StorageFolder folder)
        {
            var rawAllFiles = GetAllFiles(folder);
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
            var originalList = File.ReadAllLines(VersionFilePath);
            var originalNames = new List<string>();
            var currentList = CollectFiles(DestinationPath);
            var currentNames = new List<string>();
            var differences = new List<string>();
            IEnumerable<string> diff = originalNames.Except(currentNames);

            foreach (var name in originalList)
            {
                originalNames.Add(GetFileNameFromList(name));
            }

            foreach (var name in currentList)
            {
                currentNames.Add(GetFileNameFromList(name));
            }

            foreach (string s in diff)
            {
                differences.Add(s);
            }

            var diffNames = new List<string>();
            foreach (var file in differences)
            {
                diffNames.Add(file);
            }
            
            foreach (var file in diffNames)
            {
                if (originalNames.Any(x => x.Equals(file)) && currentNames.Any(x => x.Equals(file)))
                {
                    ModifiedFiles.Add(file);
                }
                if (originalNames.Any(x => x.Equals(file)) && !currentNames.Any(x => x.Equals(file)))
                {
                    // Fallback for the case, when destination directory is empty. In this case, all differences will be interpreted like added files
                    if (currentNames.Count == 0)
                        AddedFiles.Add(file);
                    else
                        DeletedFiles.Add(file);
                }
                if (!originalNames.Any(x => x.Equals(file)) && currentNames.Any(x => x.Equals(file)))
                {
                    AddedFiles.Add(file);
                }
            }
        }

        private void DeleteFiles(List<string> files)
        {
            foreach (var file in files)
            {
                
            }
        }

        private void UpdateFiles(List<string> files)
        {

        }

        private void AddFiles(List<string> files)
        {
            foreach (var file in files)
            {
                
            }
        }

        private string GetFileNameFromList(string s)
        {
            var t = s.Split(" || ");
            return t.First();
        }

        private void CreateVersionFile()
        {
            File.Create(VersionFilePath).Close();
        }
    }
}
