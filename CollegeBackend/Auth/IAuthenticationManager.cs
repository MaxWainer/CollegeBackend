using CollegeBackend.Objects;
using CollegeBackend.Objects.Database;

namespace CollegeBackend.Auth;

public interface IAuthenticationManager
{
    TokenizedUser? Authenticate(User user);

    Task<TokenizedUser?> AuthenticateAsync(User user);

    TokenizedUser? this[Guid token] { get; }

    Task<TokenizedUser?> GetAsync(Guid token);
}