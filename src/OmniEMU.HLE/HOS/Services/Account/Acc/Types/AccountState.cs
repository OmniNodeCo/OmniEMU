using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.HLE.HOS.Services.Account.Acc
{
    [JsonConverter(typeof(TypedStringEnumConverter<AccountState>))]
    public enum AccountState
    {
        Closed,
        Open,
    }
}
