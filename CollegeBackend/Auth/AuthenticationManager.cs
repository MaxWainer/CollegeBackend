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
        if (_users.Contains(pair => pair.Value.User.Equals(user)))
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

    public TokenizedUser? this[Guid token] => _users[token];

    public Task<TokenizedUser?> GetAsync(Guid token)
    {
        return Task.FromResult(this[token]);
    }
}