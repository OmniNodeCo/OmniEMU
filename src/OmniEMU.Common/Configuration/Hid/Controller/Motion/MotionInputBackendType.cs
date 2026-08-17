using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration.Hid.Controller.Motion
{
    [JsonConverter(typeof(TypedStringEnumConverter<MotionInputBackendType>))]
    public enum MotionInputBackendType : byte
    {
        Invalid,
        GamepadDriver,
        CemuHook,
    }
}
