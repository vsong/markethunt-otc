using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkethuntOTC.Domain;
public class ParseRule
{
    public int Id { get; }
    public int ItemId { get; }
    public string Regex { get; }
    public ItemCategory ItemCategory { get; }
    public bool GroupFreshAndUnopened { get; }
    public int Priority { get; }

    public ParseRule(int id, int itemId, string regex, ItemCategory parseGroup, bool groupFreshAndUnopened, int priority)
    {
        if (string.IsNullOrWhiteSpace(regex)) throw new ArgumentException($"{nameof(regex)} cannot be null or whitespace");

        Id = id;
        ItemId = itemId;
        Regex = regex;
        ItemCategory = parseGroup;
        GroupFreshAndUnopened = groupFreshAndUnopened;
        Priority = priority;
    }
}
