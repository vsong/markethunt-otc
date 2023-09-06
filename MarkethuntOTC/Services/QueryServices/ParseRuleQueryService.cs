using MarkethuntOTC.Domain;

namespace MarkethuntOTC.Services.QueryServices;
public class ParseRuleQueryService : IParseRuleQueryService
{
    public IEnumerable<ParseRule> GetRules(IEnumerable<ItemCategory> categories)
    {
        return new List<ParseRule>
        {
            new ParseRule(1, 123, @"\b(resp|rare emp(yrial)?)\b", ItemCategory.TreasureChest, false, 0),
            new ParseRule(2, 123, @"\b(esp|emp(yrial)?)\b", ItemCategory.TreasureChest, false, 0),

            new ParseRule(3, 456, @"\b(rare bountiful beanstalk|rare bb)\b", ItemCategory.TreasureChest, true, 0),
            new ParseRule(4, 456, @"\b(bountiful beanstalk|bb)\b", ItemCategory.TreasureChest, true, 0),
        };
    }
}
