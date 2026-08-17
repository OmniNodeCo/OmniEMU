using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ModMetadata))]
    public partial class ModMetadataJsonSerializerContext : JsonSerializerContext
    {
    }
}
