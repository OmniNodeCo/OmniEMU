using OmniEMU.Ava.Common.Locale;
using OmniEMU.Ava.UI.ViewModels;
using System.IO;

namespace OmniEMU.Ava.UI.Models
{
    public class DownloadableContentModel : BaseModel
    {
        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                OnPropertyChanged();
            }
        }

        public string TitleId { get; }
        public string ContainerPath { get; }
        public string FullPath { get; }

        public string FileName => Path.GetFileName(ContainerPath);

        public string Label =>
            Path.GetExtension(FileName)?.ToLower() == ".xci" ? $"{LocaleManager.Instance[LocaleKeys.TitleBundledDlcLabel]} {FileName}" : FileName;

        public DownloadableContentModel(string titleId, string containerPath, string fullPath, bool enabled)
        {
            TitleId = titleId;
            ContainerPath = containerPath;
            FullPath = fullPath;
            Enabled = enabled;
        }
    }
}
