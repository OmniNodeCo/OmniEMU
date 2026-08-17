using System;
using System.Reflection;

namespace OmniEMU.Common
{
    /// <summary>
    /// Product and release-channel metadata used by the in-app GitHub updater.
    /// </summary>
    public static class ReleaseInformation
    {
        private const string FlatHubChannelOwner = "flathub";
        private const string FallbackVersion = "0.1.0";

        public const string ReleaseChannelName = "stable";
        public const string ReleaseChannelOwner = "OmniNodeCo";
        public const string ReleaseChannelRepo = "OmniEMU";
        public const string ConfigName = "Config.json";

        public static bool IsValid =>
            !string.IsNullOrWhiteSpace(ReleaseChannelName) &&
            !string.IsNullOrWhiteSpace(ReleaseChannelOwner) &&
            !string.IsNullOrWhiteSpace(ReleaseChannelRepo);

        public static bool IsFlatHubBuild => ReleaseChannelOwner.Equals(FlatHubChannelOwner, StringComparison.OrdinalIgnoreCase);

        public static string Version
        {
            get
            {
                string version = Assembly.GetEntryAssembly()?
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion ?? FallbackVersion;

                // SourceRevisionId is emitted as SemVer build metadata. Keep the user-facing
                // version stable while retaining prerelease labels such as 0.2.0-rc.1.
                int metadataIndex = version.IndexOf('+');
                return metadataIndex >= 0 ? version[..metadataIndex] : version;
            }
        }
    }
}
