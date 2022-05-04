namespace CollegeBackend.Extensions;

public static class ScopeFunctions
{
    public static T Apply<T>(this T value, Action<T> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        action.Invoke(value);

        return value;
    }

    public static TValue Convert<TKey, TValue>(this TKey value, Func<TKey, TValue> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        return action.Invoke(value);
    }
}