using System.Text.Json.Serialization;

namespace MarkethuntOTC.Domain.Roots.Listing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ListingType
{
    UnopenedMap = 1,
    FreshMap = 2,
    CompletedMap = 3,
    Leech = 4,
    Tradeable = 5
}