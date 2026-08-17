using Avalonia.Interactivity;
using OmniEMU.UI.App.Common;

namespace OmniEMU.Ava.UI.Helpers
{
    public class ApplicationOpenedEventArgs : RoutedEventArgs
    {
        public ApplicationData Application { get; }

        public ApplicationOpenedEventArgs(ApplicationData application, RoutedEvent routedEvent)
        {
            Application = application;
            RoutedEvent = routedEvent;
        }
    }
}
