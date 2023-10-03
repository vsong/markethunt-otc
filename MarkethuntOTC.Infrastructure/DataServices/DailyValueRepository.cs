using MarkethuntOTC.Domain.Roots.Item;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MarkethuntOTC.Infrastructure.DataServices;

public class DailyValueRepository : IDailyValueRepository
{
    private readonly IDomainContextFactory _domainContextFactory;
    private readonly IMemoryCache _memoryCache;

    public DailyValueRepository(IDomainContextFactory domainContextFactory)
    {
        _domainContextFactory = domainContextFactory;

        _memoryCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = null
        });
    }
    
    public DailyValue GetLatestDailyValue(int itemId, DateTime before)
    {
        var values = GetDailyValues(itemId);
        var dateShifted = before + TimeSpan.FromDays(1);

        var filteredValues = values.Where(x => x.Timestamp <= dateShifted).ToList();
        return !filteredValues.Any() ? null : filteredValues.MaxBy(x => x.Timestamp);
    }

    private IReadOnlyCollection<DailyValue> GetDailyValues(int itemId)
    {
        if (_memoryCache.TryGetValue(itemId, out IReadOnlyCollection<DailyValue> dailyValues)) return dailyValues;

        using var db = _domainContextFactory.Create();
        var fetchedItem = db.Items.AsNoTracking().Include(item => item.DailyValues).SingleOrDefault(x => x.Id == itemId);
        var values = fetchedItem!.DailyValues.ToList();
        
        _memoryCache.Set(itemId, values);
        
        return values;
    }
}