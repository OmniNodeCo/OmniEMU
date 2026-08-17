using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using OmniEMU.Ava.Common;
using OmniEMU.Ava.UI.ViewModels;
using OmniEMU.Ava.UI.Windows;
using System;

namespace OmniEMU.Ava.UI.Views.Main
{
    public partial class MainViewControls : UserControl
    {
        public MainWindowViewModel ViewModel;

        public MainViewControls()
        {
            InitializeComponent();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (VisualRoot is MainWindow window)
            {
                ViewModel = window.ViewModel;
            }

            DataContext = ViewModel;
        }

        public void Sort_Checked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioButton button)
            {
                ViewModel.Sort(Enum.Parse<ApplicationSort>(button.Tag.ToString()));
            }
        }

        public void Order_Checked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioButton button)
            {
                ViewModel.Sort(button.Tag.ToString() != "Descending");
            }
        }

        private void SearchBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            ViewModel.SearchText = SearchBox.Text;
        }
    }
}
