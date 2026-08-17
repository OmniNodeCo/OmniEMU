using System.Text.Json.Serialization;

namespace OmniEMU.UI.Common.Models.Amiibo
{
    [JsonSerializable(typeof(AmiiboJson))]
    public partial class AmiiboJsonSerializerContext : JsonSerializerContext
    {
    }
}
