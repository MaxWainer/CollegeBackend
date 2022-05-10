namespace CollegeBackend.Objects;

public interface IGenericResult<TEntity, TEnum>
    where TEntity : class
    where TEnum : struct, IConvertible
{
    public TEntity? Result { get; set; }

    public TEnum? ErrorValue { get; set; }

    public bool Success => ErrorValue == null;

    public bool NotSuccess => !Success;
}