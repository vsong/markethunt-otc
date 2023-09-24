using MarkethuntOTC.Domain.Roots.ParseRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MarkethuntOTC.Infrastructure.DataServices;

public class ParseRuleRepository : IParseRuleRepository
{
    private readonly IDomainContextFactory _domainContextFactory;
    private readonly IMemoryCache _memoryCache;

    private static readonly TimeSpan ParseRuleCacheLifespan = TimeSpan.FromSeconds(5);

    public ParseRuleRepository(IDomainContextFactory domainContextFactory)
    {
        _domainContextFactory = domainContextFactory;

        _memoryCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = null
        });
    }

    public IEnumerable<ParseRule> Get(IEnumerable<ParseItemCategory> categories)
    {
        return categories.SelectMany(GetRulesInCategory);
    }

    private IEnumerable<ParseRule> GetRulesInCategory(ParseItemCategory category)
    {
        if (_memoryCache.TryGetValue<IEnumerable<ParseRule>>(category, out var cachedRules))
            return cachedRules ?? throw new ApplicationException($"Cached {category} ParseRule list should never be null");

        using var db = _domainContextFactory.Create();
        var rules = db.ParseRules
            .AsNoTracking()
            .Where(x => x.ParseItemCategory == category)
            .OrderByDescending(x => x.Priority)
            .ToList();
        
        _memoryCache.Set(category, rules, ParseRuleCacheLifespan);
        
        return rules;
    }
}