// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private async void RootButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
            var rootFolder = await folderPicker.PickSingleFolderAsync();
            if (rootFolder != null)
            {
                RootButton.Content = rootFolder.DisplayName;
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            // if InitialScan(no text file present)
                // Do initialscan
            // else (if file present)
                // Compare properties with a doc and live file system
                // Show changed folder
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // foreach changed folder
                // foreach item of it
                    // copy file from A to B
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Wait/Stop current copy process
            // Stop all threads
        }

        private async Task<IReadOnlyList<IStorageItem>> ScanRootFolder(StorageFolder folder)
        {
            var result = await folder.GetItemsAsync();
            return result;
        }

        private void ShowFolders(IReadOnlyList<IStorageItem> item)
        {
            
        }
        private void ShowFiles()
        {
            
        }

        public enum FileStates
        {
            Unchanged = 0,
            Changed = 1,
            Deleted = 2,
            Added = 3
        }
    }
}
