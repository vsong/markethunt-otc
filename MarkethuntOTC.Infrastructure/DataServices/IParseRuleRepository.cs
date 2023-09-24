using MarkethuntOTC.Domain.Roots.ParseRule;

namespace MarkethuntOTC.Infrastructure.DataServices;

public interface IParseRuleRepository
{
    IEnumerable<ParseRule> Get(IEnumerable<ParseItemCategory> categories);
}