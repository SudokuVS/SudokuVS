using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SudokuVS.Server.Models;

public class AppUser : IdentityUser
{
    [MaxLength(64)]
    public string? DisplayName { get; set; }
}
