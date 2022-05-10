namespace CollegeBackend.Objects;

public interface IGenericResult<TEntity, TEnum>
    where TEntity : class
    where TEnum : struct, IConvertible
{
    /// <summary>
    /// Result value, null if result is not success
    /// </summary>
    public TEntity? Result { get; set; }

    /// <summary>
    /// Enum-based error, null if result success
    /// </summary>
    public TEnum? ErrorValue { get; set; }

    /// <summary>
    /// Check is result succeed
    /// </summary>
    public bool Success => ErrorValue == null;

    /// <summary>
    /// Check is result not succeed
    /// </summary>
    public bool NotSuccess => !Success;
}