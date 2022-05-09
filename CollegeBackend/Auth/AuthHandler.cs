using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CollegeBackend.Auth;

public class AuthHandler : AuthenticationHandler<TokenOptions>
{
    private static readonly AuthenticateResult Unauthorized = AuthenticateResult.Fail("Unauthorized");

    private readonly IAuthenticationManager _authenticationManager;

    public AuthHandler(IOptionsMonitor<TokenOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock, IAuthenticationManager authenticationManager) : base(options, logger, encoder, clock)
    {
        _authenticationManager = authenticationManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // get raw token
        var rawToken = ResolveToken(Context.Request.Headers);

        // if not found, return that token is not valid
        if (rawToken == null) return Unauthorized;

        // try parse guid token
        if (!Guid.TryParse(rawToken, out _)) return AuthenticateResult.Fail("Invalid guid");
        
        // parse token (we are sure that it'll be valid)
        var token = Guid.Parse(rawToken);

        // get token async
        var user = await _authenticationManager.GetAsync(token);

        // if user is null, unauthorized
        if (user == null) return Unauthorized;

        // get context user
        var dbUser = user.User;

        // add claims
        var claims = new List<Claim>
        {
            // name
            new(ClaimTypes.Name, dbUser.Username),
            // role
            new(ClaimTypes.Role, dbUser.Role)
        };

        // add identity
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        // add principal
        var principal = new GenericPrincipal(identity, new[] {dbUser.Role});
        // add ticket
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        // success
        return AuthenticateResult.Success(ticket);
    }

    private static string? ResolveToken(IHeaderDictionary dictionary)
    {
        // if doesn't dictionary contains Authorization header, null
        if (!dictionary.ContainsKey("Authorization"))
            return null;

        // get Authorization header
        string authorizationHeader = dictionary["Authorization"];
        if (string.IsNullOrEmpty(authorizationHeader)) return null; // if it's null or empty, return fail

        // check is bearer auth
        if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            return null;

        // get token
        var token = authorizationHeader["bearer".Length..].Trim();

        // if it's null or empty, return fail, else out raw token
        return string.IsNullOrEmpty(token) ? null : token;
    }
}