using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OmniEMU.Modules
{
    public enum DownloadStatus
    {
        Queued,
        Resolving,
        Downloading,
        Completed,
        Failed,
        Canceled,
    }

    /// <summary>
    /// A single entry in the curated, legally redistributable homebrew catalog.
    /// Every entry points at an official project page; nothing here hosts or
    /// links to Nintendo copyrighted content.
    /// </summary>
    public class CatalogEntry
    {
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string Category { get; init; } = "Apps";
        public string License { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string PageUrl { get; init; } = string.Empty;

        /// <summary>GitHub "owner/repository" slug used to resolve the newest published release.</summary>
        public string Repo { get; init; }

        /// <summary>Exact asset file name on the latest release, when it is stable across releases.</summary>
        public string AssetName { get; init; }

        /// <summary>Preferred extension used when picking an asset from a release (for example ".zip").</summary>
        public string PreferredAssetExtension { get; init; }
    }

    /// <summary>
    /// A queued, running or finished download managed by <see cref="DownloadManager"/>.
    /// All property changes are raised on the UI thread.
    /// </summary>
    public class DownloadItem : INotifyPropertyChanged
    {
        public static readonly string[] LaunchableExtensions = { ".nro", ".nsp", ".xci", ".nca", ".nso" };

        private string _title = string.Empty;
        private string _source = string.Empty;
        private string _url = string.Empty;
        private string _savePath = string.Empty;
        private DownloadStatus _status = DownloadStatus.Queued;
        private string _statusText = "Queued";
        private double _progress;
        private bool _isProgressIndeterminate;
        private string _errorMessage = string.Empty;

        internal CancellationTokenSource Cancellation { get; set; }
        internal string ExpectedSha256 { get; set; }
        internal CatalogEntry CatalogSource { get; set; }
        internal string DirectUrl { get; set; }
        internal string DirectFileName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get => _title;
            set { _title = value ?? string.Empty; Raise(); }
        }

        public string Source
        {
            get => _source;
            set { _source = value ?? string.Empty; Raise(); }
        }

        public string Url
        {
            get => _url;
            set { _url = value ?? string.Empty; Raise(); }
        }

        /// <summary>Final location of the downloaded file inside the OmniEMU downloads folder.</summary>
        public string SavePath
        {
            get => _savePath;
            set { _savePath = value ?? string.Empty; Raise(); }
        }

        /// <summary>Best file to launch for this item: an extracted homebrew binary or the downloaded file itself.</summary>
        public string LaunchPath { get; internal set; } = string.Empty;

        public DownloadStatus Status
        {
            get => _status;
            set
            {
                if (_status == value)
                {
                    return;
                }

                _status = value;
                Raise();
                Raise(nameof(IsRunning));
                Raise(nameof(IsCompleted));
                Raise(nameof(IsFailed));
                Raise(nameof(CanCancel));
                Raise(nameof(CanRetry));
                Raise(nameof(CanOpen));
                Raise(nameof(CanLaunch));
            }
        }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value ?? string.Empty; Raise(); }
        }

        /// <summary>Completion percentage between 0 and 100.</summary>
        public double Progress
        {
            get => _progress;
            set { _progress = value; Raise(); }
        }

        public bool IsProgressIndeterminate
        {
            get => _isProgressIndeterminate;
            set { _isProgressIndeterminate = value; Raise(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value ?? string.Empty; Raise(); Raise(nameof(HasError)); }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);

        public bool IsRunning => _status is DownloadStatus.Queued or DownloadStatus.Resolving or DownloadStatus.Downloading;

        public bool IsCompleted => _status == DownloadStatus.Completed;

        public bool IsFailed => _status is DownloadStatus.Failed or DownloadStatus.Canceled;

        public bool CanCancel => IsRunning;

        public bool CanRetry => IsFailed;

        public bool CanOpen => IsCompleted && !string.IsNullOrEmpty(_savePath);

        public bool CanLaunch => IsCompleted && !string.IsNullOrEmpty(LaunchPath);

        public static bool IsLaunchableFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string extension = Path.GetExtension(path);

            foreach (string launchable in LaunchableExtensions)
            {
                if (string.Equals(extension, launchable, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void Raise([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>GitHub release payload used when an exact asset name is not pinned on a catalog entry.</summary>
    public class GithubReleaseResponse
    {
        public string TagName { get; set; }
        public List<GithubReleaseAssetResponse> Assets { get; set; } = new();
    }

    public class GithubReleaseAssetResponse
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string BrowserDownloadUrl { get; set; }
        public long Size { get; set; }
        public string Digest { get; set; }
    }
}
