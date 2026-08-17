using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.Common.Logging
{
    [JsonConverter(typeof(TypedStringEnumConverter<LogLevel>))]
    public enum LogLevel
    {
        Debug,
        Stub,
        Info,
        Warning,
        Error,
        Guest,
        AccessLog,
        Notice,
        Trace,
    }
}
