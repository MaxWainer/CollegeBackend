namespace CollegeBackend.Objects;

public interface IGenericResult<TEntity>
{
    public TEntity? Result { get; set; }

    public string? ErrorMessage { get; set; }
    
    public bool Success => ErrorMessage == null;

    public bool NotSuccess => !Success;
}