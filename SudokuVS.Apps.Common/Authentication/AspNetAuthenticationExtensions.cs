using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SudokuVS.Apps.Common.Authentication;

public static class AspNetAuthenticationExtensions
{
    public static void AddPasswordIdAuthentication(this WebApplicationBuilder builder)
    {
        string? hostname = builder.Configuration.GetValue<string>("Host");
        if (hostname == null)
        {
            throw new InvalidOperationException("Could not find passwordless client id");
        }

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddOpenIdConnect(
                openIdConnectOptions =>
                {
                    openIdConnectOptions.SignInScheme = IdentityConstants.ExternalScheme;
                    openIdConnectOptions.ResponseType = OpenIdConnectResponseType.Code;
                    openIdConnectOptions.Authority = "https://api.passwordless.id";
                    openIdConnectOptions.ClientId = hostname;
                    openIdConnectOptions.MetadataAddress = "https://api.passwordless.id/.well-known/openid-configuration";
                    openIdConnectOptions.TokenValidationParameters.ValidateAudience = true;
                    openIdConnectOptions.TokenValidationParameters.ValidAudience = hostname;
                }
            )
            .AddExternalCookie();
    }
}
