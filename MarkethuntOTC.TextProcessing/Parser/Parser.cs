using System.Text.RegularExpressions;
using MarkethuntOTC.Domain.Roots.ParseRule;
using MarkethuntOTC.Infrastructure;
using MarkethuntOTC.TextProcessing.Tokens;
using Microsoft.EntityFrameworkCore;

namespace MarkethuntOTC.TextProcessing;
public class Parser : IParser
{
    private readonly IDbContextFactory<DomainContext> _contextFactory;
    static readonly Regex priceExtractionRegex = new Regex(@"^[^0-9]*([0-9]+(.[0-9]+)?|.[0-9]+).*$", RegexOptions.Compiled | RegexOptions.Singleline);

    public Parser(IDbContextFactory<DomainContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public ParseResult Parse(Token token)
    {
        return token switch
        {
            LeechToken t => ParseLeechToken(t),
            FreshMapToken t => ParseFreshMapToken(t),
            _ => throw new NotSupportedException()
        };
    }

    private (ParseRule, Match) GetMatchingRule(Token token)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var rules = context.ParseRules.Where(x => token.GetItemCategories().Contains(x.ParseItemCategory));
        
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

        if (!TryGetPrice(freshMapToken.Text.Substring(regexMatch.Index + regexMatch.Length), out var price)) return null;

        return ParseResult.CreateSuccessful(matchedRule, freshMapToken.GetListingType(), freshMapToken.IsSelling, price, null);
    }

    private ParseResult ParseLeechToken(LeechToken leechToken)
    {
        var (matchedRule, _) = GetMatchingRule(leechToken);
        if (matchedRule == null) return ParseResult.CreateUnsuccessful();

        if (!TryGetLeechSellPrice(leechToken.Text, out var price)) return null;

        return ParseResult.CreateSuccessful(matchedRule, leechToken.GetListingType(), leechToken.IsSelling, price, null);
    }
    
    private bool TryGetLeechSellPrice(string s, out double price)
    {
        price = default;

        // virtually all sell listings are of the format "LF X leechers <price>"
        var searchAfter = s.ToLowerInvariant().IndexOf("leech");
        if (searchAfter < 0) return false;

        var match = priceExtractionRegex.Match(s.Substring(searchAfter));
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }

    private static bool TryGetPrice(string s, out double price)
    {
        price = default;

        var match = priceExtractionRegex.Match(s);
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }
}