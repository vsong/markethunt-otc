namespace MarkethuntOTC.Domain.Roots.Item;

public class Item : AggregateRoot<int>
{
    public IEnumerable<DailyValue> DailyValues { get; private set; }
}