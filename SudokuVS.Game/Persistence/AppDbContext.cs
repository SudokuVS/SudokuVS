using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Models;
using SudokuVS.Game.Models.Users;

namespace SudokuVS.Game.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserIdentityEntity> Users { get; private set; }
    public DbSet<PlayerStateEntity> PlayerStates { get; private set; }
    public DbSet<SudokuGameEntity> Games { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<SudokuGameEntity>().HasMany(e => e.Players).WithOne(s => s.Game);
}
