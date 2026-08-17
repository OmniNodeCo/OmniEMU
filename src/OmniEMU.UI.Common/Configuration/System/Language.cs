using OmniEMU.Common.Utilities;
using System.Text.Json.Serialization;

namespace OmniEMU.UI.Common.Configuration.System
{
    [JsonConverter(typeof(TypedStringEnumConverter<Language>))]
    public enum Language
    {
        Japanese,
        AmericanEnglish,
        French,
        German,
        Italian,
        Spanish,
        Chinese,
        Korean,
        Dutch,
        Portuguese,
        Russian,
        Taiwanese,
        BritishEnglish,
        CanadianFrench,
        LatinAmericanSpanish,
        SimplifiedChinese,
        TraditionalChinese,
        BrazilianPortuguese,
    }
}
