using MarkethuntOTC.Domain;
using MarkethuntOTC.Services.QueryServices;
using MarkethuntOTC.TextProcessing.Tokens;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MarkethuntOTC.TextProcessing;
public class Parser : ITokenVisitor
{
    private readonly IParseRuleQueryService _ruleQueryService;
    static readonly Regex priceExtractionRegex = new Regex(@"^[^0-9]*([0-9]+(.[0-9]+)?|.[0-9]+).*$", RegexOptions.Compiled | RegexOptions.Singleline);

    public Parser(IParseRuleQueryService ruleQueryService)
    {
        _ruleQueryService = ruleQueryService;
    }

    public ParseResults Visit(LeechToken token)
    {
        var rules = _ruleQueryService.GetRules(token.TransactionType.GetItemCategories());

        ParseRule matchedRule = null;

        foreach(var rule in rules)
        {
            var regex = new Regex(rule.Regex, RegexOptions.IgnoreCase);

            var match = regex.Match(token.Text);

            if (!match.Success) continue;

            matchedRule = rule;

            break;
        }

        if (matchedRule == null) return ParseResults.CreateUnsuccessful();

        if (!TryGetLeechSellPrice(token.Text, out var price)) return null;

        return ParseResults.CreateSuccessful(matchedRule, token.TransactionType.ToTransactionType(), token.IsSelling, price, null);
    }

    public ParseResults Visit(FreshMapToken token)
    {
        var rules = _ruleQueryService.GetRules(token.TransactionType.GetItemCategories());

        ParseRule matchedRule = null;
        Match ruleMatch = null;

        foreach (var rule in rules)
        {
            var regex = new Regex(rule.Regex, RegexOptions.IgnoreCase);

            var match = regex.Match(token.Text);

            if (!match.Success) continue;

            matchedRule = rule;
            ruleMatch = match;

            break;
        }

        if (matchedRule == null) return ParseResults.CreateUnsuccessful();

        if (!TryGetPrice(token.Text.Substring(ruleMatch.Index + ruleMatch.Length), out var price)) return null;

        return ParseResults.CreateSuccessful(
            matchedRule, 
            matchedRule.GroupFreshAndUnopened ? TransactionType.UnopenedOrFreshMap : token.TransactionType.ToTransactionType(), 
            token.IsSelling, 
            price, 
            null);
    }

    private static bool TryGetPrice(string s, out double price)
    {
        price = default;

        var match = priceExtractionRegex.Match(s);
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }

    private bool TryGetLeechSellPrice(string s, out double price)
    {
        price = default;

        var searchAfter = s.ToLowerInvariant().IndexOf("leech");
        if (searchAfter < 0) return false;

        var match = priceExtractionRegex.Match(s.Substring(searchAfter));
        if (!match.Success) return false;

        price = double.Parse(match.Groups[1].Value);
        return true;
    }
}
