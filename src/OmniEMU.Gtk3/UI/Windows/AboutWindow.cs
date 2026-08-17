using Gtk;
using OmniEMU.Common.Utilities;
using OmniEMU.UI.Common.Helper;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;

namespace OmniEMU.UI.Windows
{
    public partial class AboutWindow : Window
    {
        public AboutWindow() : base($"OmniEMU {Program.Version} - About")
        {
            Icon = new Gdk.Pixbuf(Assembly.GetAssembly(typeof(OpenHelper)), "OmniEMU.UI.Common.Resources.Logo_OmniEMU.png");
            InitializeComponent();

            _ = DownloadPatronsJson();
        }

        private async Task DownloadPatronsJson()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                _patreonNamesText.Buffer.Text = "Connection Error.";
            }

            HttpClient httpClient = new();

            try
            {
                string patreonJsonString = await httpClient.GetStringAsync("https://github.com/OmniNodeCo/OmniEMU");

                _patreonNamesText.Buffer.Text = string.Join(", ", JsonHelper.Deserialize(patreonJsonString, CommonJsonContext.Default.StringArray));
            }
            catch
            {
                _patreonNamesText.Buffer.Text = "API Error.";
            }
        }

        //
        // Events
        //
        private void OmniEMUButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://github.com/OmniNodeCo/OmniEMU");
        }

        private void AmiiboApiButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://amiiboapi.com");
        }

        private void PatreonButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://www.patreon.com/omniemu");
        }

        private void GitHubButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://github.com/OmniNodeCo/OmniEMU");
        }

        private void DiscordButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://discordapp.com/invite/N2FmfVc");
        }

        private void TwitterButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://twitter.com/OmniEMUEmu");
        }

        private void ContributorsButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://github.com/OmniNodeCo/OmniEMU/graphs/contributors?type=a");
        }

        private void ChangelogButton_Pressed(object sender, ButtonPressEventArgs args)
        {
            OpenHelper.OpenUrl("https://github.com/OmniNodeCo/OmniEMU/releases");
        }
    }
}
