using Avalonia.Media;
using OmniEMU.Ava.UI.Controls;
using OmniEMU.Ava.UI.ViewModels;
using OmniEMU.Ava.UI.Views.User;
using OmniEMU.HLE.HOS.Services.Account.Acc;
using Profile = OmniEMU.HLE.HOS.Services.Account.Acc.UserProfile;

namespace OmniEMU.Ava.UI.Models
{
    public class UserProfile : BaseModel
    {
        private readonly Profile _profile;
        private readonly NavigationDialogHost _owner;
        private byte[] _image;
        private string _name;
        private UserId _userId;
        private bool _isPointerOver;
        private IBrush _backgroundColor;

        public byte[] Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public UserId UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsPointerOver
        {
            get => _isPointerOver;
            set
            {
                _isPointerOver = value;
                OnPropertyChanged();
            }
        }

        public IBrush BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public UserProfile(Profile profile, NavigationDialogHost owner)
        {
            _profile = profile;
            _owner = owner;

            UpdateBackground();

            Image = profile.Image;
            Name = profile.Name;
            UserId = profile.UserId;
        }

        public void UpdateState()
        {
            UpdateBackground();
            OnPropertyChanged(nameof(Name));
        }

        private void UpdateBackground()
        {
            var currentApplication = Avalonia.Application.Current;
            currentApplication.Styles.TryGetResource("ControlFillColorSecondary", currentApplication.ActualThemeVariant, out object color);

            if (color is not null)
            {
                BackgroundColor = _profile.AccountState == AccountState.Open ? new SolidColorBrush((Color)color) : Brushes.Transparent;
            }
        }

        public void Recover(UserProfile userProfile)
        {
            _owner.Navigate(typeof(UserEditorView), (_owner, userProfile, true));
        }
    }
}
