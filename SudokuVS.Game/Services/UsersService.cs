using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Models.Users;
using SudokuVS.Game.Persistence;
using SudokuVS.Game.Utils;

namespace SudokuVS.Game.Services;

public class UsersService
{
    readonly AppDbContext _context;

    public UsersService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserIdentityEntity> RegisterUser(Guid userId, string name, CancellationToken cancellationToken)
    {
        UserIdentityEntity user = new() { Id = userId, Name = name };
        await _context.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public async Task<bool> IsUserRegistered(Guid userId, CancellationToken cancellationToken) => await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    public async Task<UserIdentityEntity> RequireUser(Guid userId, CancellationToken cancellationToken) => await _context.Users.RequireAsync(userId, cancellationToken);
}
