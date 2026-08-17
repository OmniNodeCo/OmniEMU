using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration.Hid.Controller
{
    [JsonConverter(typeof(TypedStringEnumConverter<StickInputId>))]
    public enum StickInputId : byte
    {
        Unbound,
        Left,
        Right,

        Count,
    }
}
