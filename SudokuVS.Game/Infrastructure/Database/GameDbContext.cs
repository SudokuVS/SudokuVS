using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Models;

namespace SudokuVS.Game.Infrastructure.Database;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<PlayerStateEntity> PlayerStates { get; private set; }
    public DbSet<SudokuGameEntity> Games { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<SudokuGameEntity>().HasMany(e => e.Players).WithOne(s => s.Game).HasForeignKey(s => s.GameId).IsRequired();
}
