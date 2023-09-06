using MarkethuntOTC.Domain;

namespace MarkethuntOTC.Services.QueryServices;

public interface IParseRuleQueryService
{
    IEnumerable<ParseRule> GetRules(IEnumerable<ItemCategory> groups);
}