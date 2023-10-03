using MarkethuntOTC.Domain.Roots.Listing;
using MarkethuntOTC.Domain.Roots.ParseRule;

namespace MarkethuntOTC.TextProcessing.Tokens;

public static class TokenExtensions
{
    private static readonly IReadOnlyDictionary<Type, IEnumerable<ParseItemCategory>> CategoryMap = new Dictionary<Type, IEnumerable<ParseItemCategory>>
    {
        {typeof(UnopenedMapToken), new HashSet<ParseItemCategory> { ParseItemCategory.ScrollCase } },
        {typeof(LeechToken), new HashSet<ParseItemCategory> { ParseItemCategory.TreasureChest } },
        {typeof(TradeableToken), new HashSet<ParseItemCategory> { ParseItemCategory.Tradeable } },
    };

    public static IEnumerable<ParseItemCategory> GetItemCategories(this Token token)
    {
        return CategoryMap[token.GetType()];
    }

    public static ListingType GetListingType(this Token token)
    {
        return token switch
        {
            UnopenedMapToken => ListingType.UnopenedMap,
            FreshMapToken => ListingType.FreshMap,
            CompletedMapToken => ListingType.CompletedMap,
            LeechToken => ListingType.Leech,
            TradeableToken => ListingType.Tradeable,
            _ => throw new NotSupportedException(token.GetType().FullName),
        };
    }
}