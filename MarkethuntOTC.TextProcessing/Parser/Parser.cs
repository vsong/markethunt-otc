using System.Text.RegularExpressions;
using log4net;
using MarkethuntOTC.Domain.Roots.ParseRule;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.Infrastructure.DataServices;
using MarkethuntOTC.TextProcessing.Tokens;
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.TextProcessing.Parser;

public class Parser : IParser
{
    private readonly IDomainContextFactory _contextFactory;
    private readonly IParseRuleRepository _parseRuleRepository;
    
    private static readonly Regex PriceExtractionRegex = new Regex(@"^[^0-9]*([0-9]+(\.[0-9]+)?|\.[0-9]+).*$", RegexOptions.Compiled | RegexOptions.Singleline);
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
            FreshMapToken t => ParseFreshMapToken(t),
            CompletedMapToken _ => ParseResult.CreateUnsuccessful(),
            TradeableToken _ => ParseResult.CreateUnsuccessful(),
            UnopenedMapToken _ => ParseResult.CreateUnsuccessful(),
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

    private ParseResult ParseFreshMapToken(FreshMapToken freshMapToken)
    {
        var (matchedRule, regexMatch) = GetMatchingRule(freshMapToken);
        if (matchedRule == null) return ParseResult.CreateUnsuccessful();

        if (!TryGetPrice(freshMapToken.Text.Substring(regexMatch.Index + regexMatch.Length), out var price)) return ParseResult.CreateUnsuccessful();

        return ParseResult.CreateSuccessful(matchedRule, freshMapToken.GetListingType(), freshMapToken.IsSelling, price, null);
    }

    private ParseResult ParseLeechToken(LeechToken leechToken)
    {
        var (matchedRule, _) = GetMatchingRule(leechToken);
        if (matchedRule == null) return ParseResult.CreateUnsuccessful();

        if (!TryGetLeechSellPrice(leechToken.Text, out var price))
        {
            Log.Debug($"Matched rule {matchedRule.Regex} but unable to find leech price");
            return ParseResult.CreateUnsuccessful();
        }

        return ParseResult.CreateSuccessful(matchedRule, leechToken.GetListingType(), leechToken.IsSelling, price, null);
    }
    
    private bool TryGetLeechSellPrice(string s, out double price)
    {
        price = default;

        // virtually all sell listings are of the format "LF X leechers <price>"
        var searchAfter = s.ToLowerInvariant().IndexOf("leech", StringComparison.InvariantCulture);
        if (searchAfter < 0) return false;

        var match = PriceExtractionRegex.Match(s.Substring(searchAfter));
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }

    private static bool TryGetPrice(string s, out double price)
    {
        price = default;

        var match = PriceExtractionRegex.Match(s);
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }
}