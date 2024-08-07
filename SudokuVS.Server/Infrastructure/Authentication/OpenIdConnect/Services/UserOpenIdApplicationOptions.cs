namespace SudokuVS.Server.Infrastructure.Authentication.OpenIdConnect.Services;

public class UserOpenIdApplicationOptions
{
    public OpenIdApplicationType ApplicationType { get; set; }
    public OpenIdClientType ClientType { get; set; }
    public OpenIdConsentType ConsentType { get; set; }
}
