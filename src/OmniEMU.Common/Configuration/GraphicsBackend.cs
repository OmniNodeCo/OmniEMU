using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration
{
    [JsonConverter(typeof(TypedStringEnumConverter<GraphicsBackend>))]
    public enum GraphicsBackend
    {
        Vulkan,
        OpenGl,
    }
}
