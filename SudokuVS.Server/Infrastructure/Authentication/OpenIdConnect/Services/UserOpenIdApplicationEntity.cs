using System.ComponentModel.DataAnnotations;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication.OpenIdConnect.Services;

public class UserOpenIdApplicationEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // EF ctor
    public UserOpenIdApplicationEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public UserOpenIdApplicationEntity(AppUser owner, string clientId, string name, OpenIdApplicationType applicationType, OpenIdConsentType consentType, string redirectUris)
    {
        CreationDate = DateTime.Now;
        Owner = owner;
        ClientId = clientId;
        Name = name;
        ApplicationType = applicationType;
        ConsentType = consentType;
        RedirectUris = redirectUris;
    }

    public Guid Id { get; private set; }
    public DateTime CreationDate { get; private set; }
    public AppUser Owner { get; private set; }

    [MaxLength(256)]
    public string ClientId { get; private set; }

    [MaxLength(256)]
    public string Name { get; private set; }

    public OpenIdApplicationType ApplicationType { get; private set; }
    public OpenIdConsentType ConsentType { get; private set; }

    [MaxLength(1024)]
    public string RedirectUris { get; private set; }
}
