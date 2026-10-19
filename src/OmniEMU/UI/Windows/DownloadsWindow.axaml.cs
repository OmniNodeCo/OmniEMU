using Avalonia.Controls;
using Avalonia.Interactivity;
using OmniEMU.Ava.UI.Helpers;
using OmniEMU.Ava.UI.ViewModels;
using OmniEMU.Modules;
using OmniEMU.UI.App.Common;
using OmniEMU.UI.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace OmniEMU.Ava.UI.Windows
{
    /// <summary>
    /// Homebrew catalog, direct URL downloader and download queue. The window is
    /// non-modal so downloads keep running while games are played.
    /// </summary>
    public partial class DownloadsWindow : StyleableWindow
    {
        private readonly MainWindow _owner;
        private readonly DownloadsWindowViewModel _viewModel;

        public DownloadsWindow(MainWindow owner)
        {
            _owner = owner;
            _viewModel = new DownloadsWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

        }

        private void SelectTab(string tab)
        {
            _viewModel.ShowTab(tab);

            SetTabActive(BrowseTabButton, tab == "browse");
            SetTabActive(DirectTabButton, tab == "direct");
            SetTabActive(QueueTabButton, tab == "queue");
        }

        private static void SetTabActive(Button button, bool active)
        {
            if (active)
            {
                if (!button.Classes.Contains("active"))
                {
                    button.Classes.Add("active");
                }
            }
            else
            {
                button.Classes.Remove("active");
            }
        }

        private void BrowseTab_Click(object sender, RoutedEventArgs e)
        {
            SelectTab("browse");
        }

        private void DirectTab_Click(object sender, RoutedEventArgs e)
        {
            SelectTab("direct");
        }

        private void QueueTab_Click(object sender, RoutedEventArgs e)
        {
            SelectTab("queue");
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }

        private void Page_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CatalogEntry entry && !string.IsNullOrEmpty(entry.PageUrl))
            {
                OpenHelper.OpenUrl(entry.PageUrl);
            }
        }

        private void DownloadCatalog_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CatalogEntry entry)
            {
                DownloadManager.EnqueueCatalog(entry);
                SelectTab("queue");
            }
        }

        private async void StartDirect_Click(object sender, RoutedEventArgs e)
        {
            if (RightsCheckBox.IsChecked != true)
            {
                await ContentDialogHelper.CreateWarningDialog(
                    "Confirmation required",
                    "Confirm that you own this software, or that it is freely and legally distributed, before starting the download.");

                return;
            }

            string url = DirectUrlBox.Text?.Trim();
            string fileName = DirectNameBox.Text?.Trim();

            if (string.IsNullOrEmpty(url))
            {
                await ContentDialogHelper.CreateErrorDialog("Enter the URL of the file you want to download.");
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DeriveFileName(url);
            }

            try
            {
                DownloadManager.EnqueueDirect(url, fileName);
            }
            catch (ArgumentException ex)
            {
                await ContentDialogHelper.CreateErrorDialog(ex.Message);
                return;
            }

            DirectNameBox.Text = fileName;
            SelectTab("queue");
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenHelper.OpenFolder(DownloadManager.GetDownloadsDirectory());
        }

        private void ClearFinished_Click(object sender, RoutedEventArgs e)
        {
            DownloadManager.ClearFinished();
        }

        private void CancelItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DownloadItem item)
            {
                DownloadManager.Cancel(item);
            }
        }

        private void RetryItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DownloadItem item)
            {
                DownloadManager.Retry(item);
            }
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DownloadItem item)
            {
                OpenHelper.LocateFile(item.SavePath);
            }
        }

        private async void LaunchItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not DownloadItem item)
            {
                return;
            }

            if (_owner.ViewModel.IsGameRunning)
            {
                await ContentDialogHelper.CreateWarningDialog(
                    "A game is running",
                    "Stop the current game before launching a downloaded title.");

                return;
            }

            string path = string.IsNullOrEmpty(item.LaunchPath) ? item.SavePath : item.LaunchPath;

            if (_owner.ApplicationLibrary.TryGetApplicationsFromFile(path, out List<ApplicationData> applications) &&
                applications.Count > 0)
            {
                Close();
                await _owner.ViewModel.LoadApplication(applications[0]);
            }
            else
            {
                await ContentDialogHelper.CreateErrorDialog(
                    "OmniEMU could not load this file as a homebrew application or game image.");
            }
        }

        private static string DeriveFileName(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                string path = Uri.UnescapeDataString(uri.AbsolutePath);
                return Path.GetFileName(path);
            }

            return string.Empty;
        }
    }
}
