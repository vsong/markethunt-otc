using MarkethuntOTC.Domain.Roots.Item;

namespace MarkethuntOTC.Infrastructure.DataServices;

public interface IDailyValueRepository
{
    DailyValue GetLatestDailyValue(int itemId, DateTime before);
}