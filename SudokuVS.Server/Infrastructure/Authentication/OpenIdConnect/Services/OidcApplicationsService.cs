using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using SudokuVS.Server.Infrastructure.Database;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication.OpenIdConnect.Services;

public class OidcApplicationsService
{
    readonly IOpenIddictApplicationManager _applicationManager;
    readonly AppDbContext _context;

    public OidcApplicationsService(IOpenIddictApplicationManager applicationManager, AppDbContext context)
    {
        _applicationManager = applicationManager;
        _context = context;
    }

    public async Task<UserOpenIdApplicationEntity> CreateApplicationForAuthorizationCodeFlowAsync(
        AppUser user,
        string name,
        UserOpenIdApplicationOptions options,
        CancellationToken cancellationToken = default
    )
    {
        string clientId = Guid.NewGuid().ToString();

        UserOpenIdApplicationEntity application = new(user, clientId, name, options.ApplicationType, options.ConsentType, string.Join(";", options.RedirectUris));
        _context.OpenIdApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        OpenIddictApplicationDescriptor descriptor = new()
        {
            DisplayName = name,
            ApplicationType = GetApplicationType(options.ApplicationType),
            ClientId = clientId,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = GetConsentType(options.ConsentType),
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Logout,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        foreach (string uri in options.RedirectUris)
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        await _applicationManager.CreateAsync(descriptor, cancellationToken);

        return application;
    }

    public async Task<IReadOnlyList<UserOpenIdApplicationEntity>> GetApplicationsAsync(AppUser user, CancellationToken cancellationToken = default) =>
        await _context.OpenIdApplications.Include(a => a.Owner).Where(a => a.Owner == user).ToListAsync(cancellationToken);

    public async Task RemoveApplicationAsync(AppUser user, string clientId, CancellationToken cancellationToken = default)
    {
        UserOpenIdApplicationEntity? application = await _context.OpenIdApplications.SingleOrDefaultAsync(a => a.Owner == user && a.ClientId == clientId, cancellationToken);
        if (application != null)
        {
            _context.OpenIdApplications.Remove(application);
            await _context.SaveChangesAsync(cancellationToken);
        }

        object? openIdDictApplication = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        if (openIdDictApplication != null)
        {
            await _applicationManager.DeleteAsync(openIdDictApplication, cancellationToken);
        }
    }


    static string GetApplicationType(OpenIdApplicationType applicationType) =>
        applicationType switch
        {
            OpenIdApplicationType.Web => OpenIddictConstants.ApplicationTypes.Web,
            OpenIdApplicationType.Native => OpenIddictConstants.ApplicationTypes.Native,
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType), applicationType, null)
        };

    static string GetConsentType(OpenIdConsentType consentType) =>
        consentType switch
        {
            OpenIdConsentType.Explicit => OpenIddictConstants.ConsentTypes.Explicit,
            OpenIdConsentType.External => OpenIddictConstants.ConsentTypes.External,
            OpenIdConsentType.Implicit => OpenIddictConstants.ConsentTypes.Implicit,
            OpenIdConsentType.Systematic => OpenIddictConstants.ConsentTypes.Systematic,
            _ => throw new ArgumentOutOfRangeException(nameof(consentType), consentType, null)
        };
}
