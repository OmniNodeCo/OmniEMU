using Gtk;
using OmniEMU.UI.Common.Helper;
using System.Reflection;

namespace OmniEMU.UI.Windows
{
    public partial class AboutWindow : Window
    {
        private const string ProjectUrl = "https://github.com/OmniNodeCo/OmniEMU";

        public AboutWindow() : base($"OmniEMU {Program.Version} · About")
        {
            Icon = new Gdk.Pixbuf(Assembly.GetAssembly(typeof(OpenHelper)), "OmniEMU.UI.Common.Resources.Logo_OmniEMU.png");
            InitializeComponent();

            _patreonNamesLabel.Text = "Project stewardship:";
            _patreonNamesText.Buffer.Text = "Created and maintained by OmniNodeCo.";
        }

        private void OmniEMUButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl(ProjectUrl);

        private void AmiiboApiButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl("https://amiiboapi.com");

        private void PatreonButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl(ProjectUrl);

        private void GitHubButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl(ProjectUrl);

        // The Discord control stays hidden until OmniNodeCo publishes an official invite.
        private void DiscordButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl(ProjectUrl);

        private void TwitterButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl(ProjectUrl);

        private void ContributorsButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl($"{ProjectUrl}/graphs/contributors?type=a");

        private void ChangelogButton_Pressed(object sender, ButtonPressEventArgs args) => OpenHelper.OpenUrl($"{ProjectUrl}/releases");
    }
}
