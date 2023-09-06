namespace MarkethuntOTC.Domain;

public class Transaction
{
    public int Id { get; }
    public ulong MessageId { get; }
    public int ItemId { get; }
    public TransactionType TransactionType { get; }
    public bool IsSelling { get; }
    public double SbPrice { get; }
    public int? Amount { get; }

    public Transaction(int id, ulong messageId, int itemId, TransactionType transactionType, bool isSelling, double sbPrice, int? amount)
    {
        Id = id;
        MessageId = messageId;
        ItemId = itemId;
        TransactionType = transactionType;
        IsSelling = isSelling;
        SbPrice = sbPrice;
        Amount = amount;
    }
}