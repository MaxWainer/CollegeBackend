namespace CollegeBackend.Extensions;

public static class CollectionExtensions
{
    public static bool ContainsPredicate<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        foreach (var t in collection)
        {
            if (predicate.Invoke(t))
            {
                return true;
            }
        }

        return false;
    }
    
    public static IServiceCollection AddDiService<T>(this IServiceCollection collection, T instance)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
    
        // add it via ServiceDescriptor
        collection.Add(
            new ServiceDescriptor(typeof(T), instance)
        );

        return collection;
    }
}