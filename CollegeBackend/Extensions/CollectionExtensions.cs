using System.Collections;

namespace CollegeBackend.Extensions;

public static class CollectionExtensions
{
    public static bool ContainsPredicate<T>(this ICollection<T> collection, Func<T, bool> predicate)
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
}