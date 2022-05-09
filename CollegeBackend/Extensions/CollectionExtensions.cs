namespace CollegeBackend.Extensions;

public static class CollectionExtensions
{
    public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        return collection.Any(predicate.Invoke);
    }

    public static IServiceCollection InjectService<T>(this IServiceCollection collection, T instance)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        // add it via ServiceDescriptor
        collection.Add(
            new ServiceDescriptor(typeof(T), instance)
        );

        return collection;
    }
}