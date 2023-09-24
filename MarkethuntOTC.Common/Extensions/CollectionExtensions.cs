namespace MarkethuntOTC.Common.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<(TSource Item, int Index)> WithIndex<TSource>(this IEnumerable<TSource> list)
    {
        return list.Select((item, index) => (item, index));
    }
}