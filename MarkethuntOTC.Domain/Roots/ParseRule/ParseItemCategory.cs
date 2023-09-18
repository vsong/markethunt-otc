using System.Text.Json.Serialization;

namespace MarkethuntOTC.Domain.Roots.ParseRule;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParseItemCategory
{
    ScrollCase = 1,
    TreasureChest = 2,
    Tradeable = 3
}