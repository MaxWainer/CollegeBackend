namespace CollegeBackend.Extensions;

public static class ScopeFunctions
{
    public static TValue Apply<TValue>(this TValue value, Action<TValue> action)
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