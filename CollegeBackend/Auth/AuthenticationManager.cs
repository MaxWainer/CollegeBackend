using System.Collections.Concurrent;
using CollegeBackend.Extensions;
using CollegeBackend.Objects;
using CollegeBackend.Objects.Database;

namespace CollegeBackend.Auth;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly IDictionary<Guid, TokenizedUser> _users = new ConcurrentDictionary<Guid, TokenizedUser>();

    public TokenizedUser? Authenticate(User user)
    {
        // check is user exists
        if (_users.Contains((_, value) => value.User.PassportId == user.PassportId))
        {
            return null;
        }

        // generate token
        var generated = Guid.NewGuid();

        // create new user
        var tokenizedUser = new TokenizedUser
        {
            Token = generated,
            User = user
        };

        // add in cache
        _users.Add(generated, tokenizedUser);

        // return it
        return tokenizedUser;
    }

    public Task<TokenizedUser?> AuthenticateAsync(User user)
    {
        return Task.FromResult(Authenticate(user));
    }

    public bool ClearAuthentication(Guid guid)
    {
        return _users.Remove(guid);
    }

    public TokenizedUser? this[Guid token]
    {
        get
        {
            KeyValuePair<Guid, TokenizedUser>? result = _users.FirstOrDefault(pair => pair.Key == token);

            return result?.Value;
        }
    }

    public Task<TokenizedUser?> GetAsync(Guid token)
    {
        return Task.FromResult(this[token]);
    }

    public Task<TokenizedUser?> GetAsyncByUser(User user)
    {
        KeyValuePair<Guid, TokenizedUser>? result = _users.FirstOrDefault(pair => pair.Value.User.PassportId == user.PassportId);

        return Task.FromResult(result?.Value);
    }
}