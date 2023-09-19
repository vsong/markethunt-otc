using System.Text.RegularExpressions;
using MarkethuntOTC.Common.Extensions;

namespace MarkethuntOTC.Domain.Roots.ParseRule;

public class ParseRule : AggregateRoot<int>
{
    private string _regex;
    
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
        return new Regex(Regex, RegexOptions.IgnoreCase);
    }
}