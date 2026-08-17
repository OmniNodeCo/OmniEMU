using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmniEMU.UI.Common.Models.Github
{
    public class GithubReleasesJsonResponse
    {
        public string Name { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        public List<GithubReleaseAssetJsonResponse> Assets { get; set; } = new();
    }
}
