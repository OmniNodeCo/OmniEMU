using OmniEMU.Common.Configuration.Hid.Controller;
using OmniEMU.Common.Configuration.Hid.Keyboard;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Configuration.Hid
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(InputConfig))]
    [JsonSerializable(typeof(StandardKeyboardInputConfig))]
    [JsonSerializable(typeof(StandardControllerInputConfig))]
    public partial class InputConfigJsonSerializerContext : JsonSerializerContext
    {
    }
}
