using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using SudokuVS.Server.Exceptions;
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

    public async Task<UserOpenIdApplicationEntity> CreateNewApplication(
        AppUser user,
        string name,
        UserOpenIdApplicationOptions options,
        CancellationToken cancellationToken = default
    )
    {
        string clientId = Guid.NewGuid().ToString();

        UserOpenIdApplicationEntity application = new(user, clientId, name);
        _context.OpenIdApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        await _applicationManager.CreateAsync(
            new OpenIddictApplicationDescriptor
            {
                DisplayName = name,
                ApplicationType = GetApplicationType(options.ApplicationType),
                ClientId = clientId,
                ClientSecret = GenerateClientSecret(),
                ClientType = GetClientType(options.ClientType),
                ConsentType = GetConsentType(options.ConsentType)
            },
            cancellationToken
        );

        return application;
    }

    public async Task<IReadOnlyList<UserOpenIdApplicationEntity>> GetApplications(AppUser user, CancellationToken cancellationToken = default) =>
        await _context.OpenIdApplications.Include(a => a.Owner).Where(a => a.Owner == user).ToListAsync(cancellationToken);

    public async Task RemoveApplication(AppUser user, string clientId, CancellationToken cancellationToken = default)
    {
        UserOpenIdApplicationEntity? application = await _context.OpenIdApplications.SingleOrDefaultAsync(a => a.Owner == user && a.ClientId == clientId, cancellationToken);
        if (application == null)
        {
            throw new NotFoundException("OpenID application not found.");
        }

        _context.OpenIdApplications.Remove(application);
        await _context.SaveChangesAsync(cancellationToken);
    }


    static string GetApplicationType(OpenIdApplicationType applicationType) =>
        applicationType switch
        {
            OpenIdApplicationType.Web => OpenIddictConstants.ApplicationTypes.Web,
            OpenIdApplicationType.Native => OpenIddictConstants.ApplicationTypes.Native,
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType), applicationType, null)
        };

    static string GetClientType(OpenIdClientType clientType) =>
        clientType switch
        {
            OpenIdClientType.Public => OpenIddictConstants.ClientTypes.Public,
            OpenIdClientType.Confidential => OpenIddictConstants.ClientTypes.Confidential,
            _ => throw new ArgumentOutOfRangeException(nameof(clientType), clientType, null)
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

    static string GenerateClientSecret()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] data = new byte[32];
        rng.GetBytes(data);
        return Convert.ToBase64String(data);
    }
}
