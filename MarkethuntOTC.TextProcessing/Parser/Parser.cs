using System.Globalization;
using System.Text.RegularExpressions;
using log4net;
using MarkethuntOTC.Domain.Roots.ParseRule;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.Infrastructure.DataServices;
using MarkethuntOTC.TextProcessing.Tokens;
using MarkethuntOTC.Common.Extensions;

namespace MarkethuntOTC.TextProcessing.Parser;

public class Parser : IParser
{
    private readonly IDomainContextFactory _contextFactory;
    private readonly IParseRuleRepository _parseRuleRepository;
    
    private static readonly Regex SimplePriceExtractionRegex = new (@"([0-9]+(\.[0-9]+)?|\.[0-9]+)", RegexOptions.Compiled);
    private static readonly Regex PriceExtractionRegex = new (@"(\d+,\d\d\d|\d+\.\d+|\.\d+|\d+)( ?(k\b|ksb))?", RegexOptions.Compiled);
    private static readonly Regex AmountExtractionRegex = new (@"(\d+(\.\d+)? ?k\b|\d+)", RegexOptions.Compiled);
    private static readonly Regex AlternateLeechIdentificationRegex = new(@"lf \d ?l", RegexOptions.Compiled);
    
    private static readonly ILog Log = LogManager.GetLogger(typeof(Parser));
    
    public Parser(IDomainContextFactory contextFactory, IParseRuleRepository parseRuleRepository)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _parseRuleRepository = parseRuleRepository ?? throw new ArgumentNullException(nameof(parseRuleRepository));
    }

    public ParseResult Parse(Token token)
    {
        return token switch
        {
            LeechToken t => ParseLeechToken(t),
            FreshMapToken t => ParseResult.CreateUnsuccessful(),
            CompletedMapToken t => ParseResult.CreateUnsuccessful(),
            TradeableToken t => ParseTradeableToken(t),
            UnopenedMapToken t => ParseUnopenedMapToken(t),
            _ => throw new NotSupportedException()
        };
    }

    private (ParseRule, Match) GetMatchingRule(Token token)
    {
        using var db = _contextFactory.Create();

        var itemCategories = token.GetItemCategories();

        var rules = _parseRuleRepository.Get(itemCategories).ToList();
        
        ParseRule matchedRule = null;
        Match regexMatch = null;

        foreach(var rule in rules)
        {
            var regex = rule.GetCompiledRegex();

            var match = regex.Match(token.Text);

            if (!match.Success) continue;

            matchedRule = rule;
            regexMatch = match;

            break;
        }

        return (matchedRule, regexMatch);
    }

    private ParseResult ParseLeechToken(LeechToken leechToken)
    {
        var (matchedRule, _) = GetMatchingRule(leechToken);
        if (matchedRule == null) return ParseResult.CreateUnsuccessful();

        if (!TryGetLeechSellPrice(leechToken.Text, out var price)) return ParseResult.CreateUnsuccessful();

        return ParseResult.CreateSuccessful(matchedRule, leechToken.GetListingType(), leechToken.IsSelling, price, null);
    }
    
    private ParseResult ParseUnopenedMapToken(UnopenedMapToken unopenedMapToken)
    {
        var (matchedRule, regexMatch) = GetMatchingRule(unopenedMapToken);
        if (matchedRule == null) return ParseResult.CreateUnsuccessful();

        if (!TryGetSimplePrice(unopenedMapToken.Text[regexMatch.Index..], out var price)) return ParseResult.CreateUnsuccessful();

        return ParseResult.CreateSuccessful(matchedRule, unopenedMapToken.GetListingType(), unopenedMapToken.IsSelling, price, null);
    }
    
    private ParseResult ParseTradeableToken(TradeableToken tradeableToken)
    {
        var (matchedRule, regexMatch) = GetMatchingRule(tradeableToken);
        if (matchedRule == null) return ParseResult.CreateUnsuccessful();

        if (!TryGetTradeablePrice(
                tradeableToken.Text[..regexMatch.Index],
                tradeableToken.Text[(regexMatch.Index + regexMatch.Length)..],
                out var price,
                out var amount))
        {
            return ParseResult.CreateUnsuccessful();
        }
        
        return ParseResult.CreateSuccessful(matchedRule, tradeableToken.GetListingType(), tradeableToken.IsSelling, price, amount);
    }

    private bool TryGetLeechSellPrice(string s, out double price)
    {
        price = default;

        var searchAfter = s.IndexOf("leech", StringComparison.InvariantCulture);
        if (searchAfter < 0) searchAfter = s.IndexOf(AlternateLeechIdentificationRegex);
        if (searchAfter < 0) return false;

        var match = SimplePriceExtractionRegex.Match(s[searchAfter..]);
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }

    private static bool TryGetSimplePrice(string s, out double price)
    {
        price = default;

        var match = SimplePriceExtractionRegex.Match(s);
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }
    
    private static bool TryGetTradeablePrice(string pre, string post, out double price, out int? amount)
    {
        price = default;
        amount = null;

        var amountMatch = AmountExtractionRegex.Match(pre);
        if (amountMatch.Success)
        {
            var amountRaw = amountMatch.Groups[1].Value.Replace(" ", "").Replace("k", "e3"); 
            if (double.TryParse(amountRaw, out var parsedAmount)) amount = (int)parsedAmount;
        }

        var priceMatch = PriceExtractionRegex.Match(post);
        if (!priceMatch.Success) return false;
        var priceRaw = priceMatch.Groups[1].Value.Replace(" ", "") + (priceMatch.Groups[2].Success ? "e3" : "");
        return double.TryParse(priceRaw, out price);
    }
}