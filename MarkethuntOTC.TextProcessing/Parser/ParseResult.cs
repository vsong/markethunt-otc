using MarkethuntOTC.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkethuntOTC.Domain.Roots.Listing;
using MarkethuntOTC.Domain.Roots.ParseRule;

namespace MarkethuntOTC.TextProcessing;
public class ParseResult
{
    public bool Successful { get; }
    public ParseRule MatchedParseRule { get; }
    public ListingType ListingType { get; }
    public bool IsSelling { get; }
    public double SbPrice { get; }
    public int? Amount { get; }

    private ParseResult(bool successful, ParseRule matchedParseRule, ListingType listingType, bool isSelling, double sbPrice, int? amount)
    {
        Successful = successful;
        MatchedParseRule = matchedParseRule;
        ListingType = listingType;
        IsSelling = isSelling;
        SbPrice = sbPrice;
        Amount = amount;
    }

    public static ParseResult CreateSuccessful(ParseRule matchedParseRule, ListingType transactionType, bool isSelling, double sbPrice, int? amount)
    {
        return new ParseResult(true, matchedParseRule, transactionType, isSelling, sbPrice, amount);
    }

    public static ParseResult CreateUnsuccessful()
    {
        return new ParseResult(false, null, default, default, default, null);
    }
}
