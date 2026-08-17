using System.Reflection;

namespace OmniEMU.Common
{
    // DO NOT EDIT, filled by CI
    public static class ReleaseInformation
    {
        private const string FlatHubChannelOwner = "flathub";

        private const string BuildVersion = "%%OMNIEMU_BUILD_VERSION%%";
        private const string BuildGitHash = "%%OMNIEMU_BUILD_GIT_HASH%%";
        private const string ReleaseChannelName = "%%OMNIEMU_TARGET_RELEASE_CHANNEL_NAME%%";
        private const string ConfigFileName = "%%OMNIEMU_CONFIG_FILE_NAME%%";

        public const string ReleaseChannelOwner = "%%OMNIEMU_TARGET_RELEASE_CHANNEL_OWNER%%";
        public const string ReleaseChannelRepo = "%%OMNIEMU_TARGET_RELEASE_CHANNEL_REPO%%";

        public static string ConfigName => !ConfigFileName.StartsWith("%%") ? ConfigFileName : "Config.json";

        public static bool IsValid =>
            !BuildGitHash.StartsWith("%%") &&
            !ReleaseChannelName.StartsWith("%%") &&
            !ReleaseChannelOwner.StartsWith("%%") &&
            !ReleaseChannelRepo.StartsWith("%%") &&
            !ConfigFileName.StartsWith("%%");

        public static bool IsFlatHubBuild => IsValid && ReleaseChannelOwner.Equals(FlatHubChannelOwner);

        public static string Version => IsValid ? BuildVersion : Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}
