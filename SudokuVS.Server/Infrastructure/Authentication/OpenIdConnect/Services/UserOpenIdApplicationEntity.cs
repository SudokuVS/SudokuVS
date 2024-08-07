using Microsoft.EntityFrameworkCore;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication.OpenIdConnect.Services;

[Index(nameof(Owner), nameof(ClientId), IsUnique = true)]
public class UserOpenIdApplicationEntity
{
    public UserOpenIdApplicationEntity(AppUser owner, string clientId, string name)
    {
        CreationDate = DateTime.Now;
        Owner = owner;
        Name = name;
        ClientId = clientId;
    }

    public Guid Id { get; set; }
    public DateTime CreationDate { get; set; }
    public AppUser Owner { get; set; }
    public string ClientId { get; set; }
    public string Name { get; set; }
}
