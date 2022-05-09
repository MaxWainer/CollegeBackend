namespace CollegeBackend.Extensions;

public static class CollectionExtensions
{
    public static bool Contains<TEntity>(this IEnumerable<TEntity> collection, Func<TEntity, bool> predicate)
    {
        return collection.Any(predicate.Invoke);
    }

    public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        Func<TKey, TValue, bool> predicate)
    {
        return dictionary.Any(pair => predicate.Invoke(pair.Key, pair.Value));
    }

    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        Action<TKey, TValue> action)
    {
        foreach (var (key, value) in dictionary) action.Invoke(key, value);
    }
}