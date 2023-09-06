using MarkethuntOTC.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarkethuntOTC.TextProcessing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TokenTransactionType
{
    UnopenedMap,
    FreshMap,
    CompletedMap,
    Leech,
    Tradeable
}

public static class TokenTransactionTypeExtensions
{
    private static readonly IReadOnlyDictionary<TokenTransactionType, IEnumerable<ItemCategory>> _categoryMap = new Dictionary<TokenTransactionType, IEnumerable<ItemCategory>>
    {
        {TokenTransactionType.UnopenedMap, new HashSet<ItemCategory> { ItemCategory.ScrollCase, ItemCategory.TreasureChest } },
        {TokenTransactionType.FreshMap, new HashSet<ItemCategory> { ItemCategory.TreasureChest } },
        {TokenTransactionType.CompletedMap, new HashSet<ItemCategory> { ItemCategory.TreasureChest } },
        {TokenTransactionType.Leech, new HashSet<ItemCategory> { ItemCategory.TreasureChest } },
        {TokenTransactionType.Tradeable, new HashSet<ItemCategory> { ItemCategory.Tradeable } },
    };

    public static IEnumerable<ItemCategory> GetItemCategories(this TokenTransactionType transactionType)
    {
        return _categoryMap[transactionType];
    }

    public static TransactionType ToTransactionType(this TokenTransactionType transactionType)
    {
        return transactionType switch
        {
            TokenTransactionType.UnopenedMap => TransactionType.UnopenedMap,
            TokenTransactionType.FreshMap => TransactionType.FreshMap,
            TokenTransactionType.CompletedMap => TransactionType.CompletedMap,
            TokenTransactionType.Leech => TransactionType.Leech,
            TokenTransactionType.Tradeable => TransactionType.Tradeable,
            _ => throw new NotSupportedException(nameof(transactionType)),
        };
    }
}