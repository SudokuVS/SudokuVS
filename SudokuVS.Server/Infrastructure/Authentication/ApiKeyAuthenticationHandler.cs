using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication;

public class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
    public static string HeaderName => HeaderNames.Authorization;
}

class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeySchemeOptions>
{
    readonly UserManager<AppUser> _userManager;
    readonly ApiKeyService _apiKeyService;

    public ApiKeyAuthenticationHandler(
        ApiKeyService apiKeyService,
        UserManager<AppUser> userManager,
        IOptionsMonitor<ApiKeySchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeySchemeOptions.HeaderName, out StringValues authHeaderValue))
        {
            return AuthenticateResult.Fail("Authorization header could not be found.");
        }

        AppUser? user = await _apiKeyService.FindUserAsync(authHeaderValue.ToString());
        if (user is null)
        {
            return AuthenticateResult.Fail("Bad api key.");
        }

        IList<Claim> claims = await _userManager.GetClaimsAsync(user);
        ClaimsIdentity identity = new(claims, ApiKeyConstants.AuthenticationType);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
