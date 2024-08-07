using OpenIddict.Abstractions;

namespace SudokuVS.Server.Infrastructure.Authentication.Oidc;

public class OidcApplicationsWorker : IHostedService
{
    readonly IServiceProvider _serviceProvider;

    public OidcApplicationsWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IOpenIddictApplicationManager manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("service-worker", cancellationToken) is null)
        {
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "service-worker",
                    ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                    }
                },
                cancellationToken
            );
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
