namespace CollegeBackend.Objects;

public class GenericResult<TEntity, TEnum> : IGenericResult<TEntity, TEnum>
    where TEntity : class
    where TEnum : struct, IConvertible
{
    public TEntity? Result { get; set; }
    public TEnum? ErrorValue { get; set; }
}