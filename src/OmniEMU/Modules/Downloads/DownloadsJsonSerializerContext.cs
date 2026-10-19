using System.Text.Json.Serialization;

namespace OmniEMU.Modules
{
    [JsonSerializable(typeof(GithubReleaseResponse))]
    internal partial class DownloadsJsonSerializerContext : JsonSerializerContext
    {
    }
}
