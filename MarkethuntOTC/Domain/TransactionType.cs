using System.Text.Json.Serialization;

namespace MarkethuntOTC.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType
{
    UnopenedMap,
    FreshMap,
    UnopenedOrFreshMap,
    CompletedMap,
    Leech,
    Tradeable
}