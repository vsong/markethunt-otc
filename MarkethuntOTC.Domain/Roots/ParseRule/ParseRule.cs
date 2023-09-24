using System.Text.RegularExpressions;
using MarkethuntOTC.Common.Extensions;

namespace MarkethuntOTC.Domain.Roots.ParseRule;

public class ParseRule : AggregateRoot<int>
{
    private string _regex;
    private Regex _compiledRegexCache = null;
    
    public int ItemId { get; private set; }

    public string Regex
    {
        get => _regex;
        set
        {
            value.ThrowIfNullOrWhitespace();
            _regex = value;
        }
    }

    public ParseItemCategory ParseItemCategory { get; set; }
    public int Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ParseRule(int itemId, string regex, ParseItemCategory parseItemCategory, int priority, DateTime? startDate, DateTime? endDate)
    {
        Id = default;
        ItemId = itemId;
        Regex = regex;
        ParseItemCategory = parseItemCategory;
        Priority = priority;
        StartDate = startDate;
        EndDate = endDate;
    }

    public Regex GetCompiledRegex()
    {
        if (_compiledRegexCache != null) return _compiledRegexCache;

        _compiledRegexCache = new Regex(Regex, RegexOptions.Compiled);
        return _compiledRegexCache;
    }
}