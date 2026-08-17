using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration
{
    [JsonConverter(typeof(TypedStringEnumConverter<ScalingFilter>))]
    public enum ScalingFilter
    {
        Bilinear,
        Nearest,
        Fsr,
    }
}
