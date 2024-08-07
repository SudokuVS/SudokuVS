using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication.ApiKey;

public class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
    public static string HeaderName => HeaderNames.Authorization;
}

class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeySchemeOptions>
{
    readonly ApiKeyService _apiKeyService;
    readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;

    public ApiKeyAuthenticationHandler(
        ApiKeyService apiKeyService,
        IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
        IOptionsMonitor<ApiKeySchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        _apiKeyService = apiKeyService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
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

        ClaimsPrincipal principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
