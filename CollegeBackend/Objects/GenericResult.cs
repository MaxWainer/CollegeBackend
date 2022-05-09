namespace CollegeBackend.Objects;

public class GenericResult<TEntity> : IGenericResult<TEntity> where TEntity : class
{
    public TEntity? Result { get; set; }
    public string? ErrorMessage { get; set; }
}