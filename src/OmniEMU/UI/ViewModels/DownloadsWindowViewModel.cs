using OmniEMU.Modules;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace OmniEMU.Ava.UI.ViewModels
{
    public class DownloadsWindowViewModel : BaseModel
    {
        private bool _showBrowse = true;
        private bool _showDirect;
        private bool _showQueue;
        private bool _queueEmpty = true;
        private string _searchText = string.Empty;

        public DownloadsWindowViewModel()
        {
            ApplyFilter(string.Empty);
            QueueEmpty = Downloads.Count == 0;

            Downloads.CollectionChanged += Downloads_CollectionChanged;
        }

        /// <summary>Filtered homebrew catalog entries shown on the Browse tab.</summary>
        public ObservableCollection<CatalogEntry> VisibleCatalog { get; } = new();

        /// <summary>Live download queue shared with <see cref="DownloadManager"/>.</summary>
        public ObservableCollection<DownloadItem> Downloads => DownloadManager.Items;

        public bool ShowBrowse
        {
            get => _showBrowse;
            private set { _showBrowse = value; OnPropertyChanged(); }
        }

        public bool ShowDirect
        {
            get => _showDirect;
            private set { _showDirect = value; OnPropertyChanged(); }
        }

        public bool ShowQueue
        {
            get => _showQueue;
            private set { _showQueue = value; OnPropertyChanged(); }
        }

        public bool QueueEmpty
        {
            get => _queueEmpty;
            private set { _queueEmpty = value; OnPropertyChanged(); }
        }

        /// <summary>Live search text for the Browse tab; filtering happens on set.</summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value ?? string.Empty;
                ApplyFilter(_searchText);
            }
        }

        public void ShowTab(string tab)
        {
            ShowBrowse = tab == "browse";
            ShowDirect = tab == "direct";
            ShowQueue = tab == "queue";
        }

        public void ApplyFilter(string search)
        {
            string query = search?.Trim() ?? string.Empty;

            VisibleCatalog.Clear();

            foreach (CatalogEntry entry in HomebrewCatalog.Entries)
            {
                if (Matches(entry, query))
                {
                    VisibleCatalog.Add(entry);
                }
            }
        }

        private static bool Matches(CatalogEntry entry, string query)
        {
            if (query.Length == 0)
            {
                return true;
            }

            return Contains(entry.Title, query) ||
                   Contains(entry.Author, query) ||
                   Contains(entry.Category, query) ||
                   Contains(entry.License, query) ||
                   Contains(entry.Description, query);
        }

        private static bool Contains(string value, string query)
        {
            return value != null && value.Contains(query, System.StringComparison.OrdinalIgnoreCase);
        }

        private void Downloads_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            QueueEmpty = Downloads.Count == 0;
        }
    }
}
