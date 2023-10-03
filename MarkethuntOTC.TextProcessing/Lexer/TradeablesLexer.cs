using System.Text.RegularExpressions;
using MarkethuntOTC.TextProcessing.Tokens;

namespace MarkethuntOTC.TextProcessing.Lexer;

public class TradeablesLexer : ILexer
{
    private static readonly Regex SellRegex = new Regex(@"^[~*_]*(s>|s |sell|wts)", RegexOptions.Compiled);
    private static readonly Regex BuyRegex = new Regex(@"^[~*_]*(b>|b |buy|wtb)", RegexOptions.Compiled);
    private static readonly Regex TradeRegex = new Regex(@"^[~*_]*(t>|t |trade?ing|trade)", RegexOptions.Compiled);

    private enum ListingType
    {
        Sell,
        Buy,
        Trade
    }
    
    public IEnumerable<Token> Tokenize(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        ListingType? latestListingTypeFound = null;
        
        foreach (var line in lines)
        {
            if (SellRegex.IsMatch(line))
            {
                latestListingTypeFound = ListingType.Sell;
                yield return new TradeableToken(true, line);
                continue;
            }
            
            if (BuyRegex.IsMatch(line))
            {
                latestListingTypeFound = ListingType.Buy;
                yield return new TradeableToken(false, line);
                continue;
            }
            
            if (TradeRegex.IsMatch(line))
            {
                latestListingTypeFound = ListingType.Trade;
                continue;
            }

            if (latestListingTypeFound == ListingType.Sell)
            {
                yield return new TradeableToken(true, line);
                continue;
            }
            
            if (latestListingTypeFound == ListingType.Buy)
            {
                yield return new TradeableToken(false, line);
            }
        }
    }
}