namespace MarkethuntOTC.Domain.Roots.Listing;

public class Listing : AggregateRoot<int>
{
    public Listing(int itemId, double sbPrice, ListingType listingType, bool isSelling, int? amount,
        ulong messageId, int parseRuleId, DateTime timestamp)
    {
        Id = default;
        ItemId = itemId;
        SbPrice = sbPrice;
        ListingType = listingType;
        IsSelling = isSelling;
        Amount = amount;
        MessageId = messageId;
        ParseRuleId = parseRuleId;
        Timestamp = timestamp;
    }

    public int ItemId { get; set; }
    public double SbPrice { get; set; }
    public ListingType ListingType { get; set; }
    public bool IsSelling { get; set; }
    public int? Amount { get; set; }
    public ulong MessageId { get; set; }
    public int ParseRuleId { get; set; }
    public DateTime Timestamp { get; set; }
}