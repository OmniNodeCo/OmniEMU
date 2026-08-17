using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration
{
    [JsonConverter(typeof(TypedStringEnumConverter<MemoryManagerMode>))]
    public enum MemoryManagerMode : byte
    {
        SoftwarePageTable,
        HostMapped,
        HostMappedUnsafe,
    }
}
