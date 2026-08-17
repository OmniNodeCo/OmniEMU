using System.Text.Json.Serialization;

namespace OmniEMU.Common.Logging
{
    [JsonSerializable(typeof(LogEventArgsJson))]
    internal partial class LogEventJsonSerializerContext : JsonSerializerContext
    {
    }
}
