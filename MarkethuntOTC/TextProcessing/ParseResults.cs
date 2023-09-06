using MarkethuntOTC.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkethuntOTC.TextProcessing;
public class ParseResults
{
    public bool Successful { get; }
    public ParseRule? MatchedParseRule { get; }
    public TransactionType TransactionType { get; }
    public bool IsSelling { get; }
    public double SbPrice { get; }
    public int? Amount { get; }

    private ParseResults(bool successful, ParseRule? matchedParseRule, TransactionType transactionType, bool isSelling, double sbPrice, int? amount)
    {
        Successful = successful;
        MatchedParseRule = matchedParseRule;
        TransactionType = transactionType;
        IsSelling = isSelling;
        SbPrice = sbPrice;
        Amount = amount;
    }

    public static ParseResults CreateSuccessful(ParseRule matchedParseRule, TransactionType transactionType, bool isSelling, double sbPrice, int? amount)
    {
        return new ParseResults(true, matchedParseRule, transactionType, isSelling, sbPrice, amount);
    }

    public static ParseResults CreateUnsuccessful()
    {
        return new ParseResults(false, null, default, default, default, null);
    }
}
