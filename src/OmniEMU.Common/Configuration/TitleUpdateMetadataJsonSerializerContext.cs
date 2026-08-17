using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(TitleUpdateMetadata))]
    public partial class TitleUpdateMetadataJsonSerializerContext : JsonSerializerContext
    {
    }
}
