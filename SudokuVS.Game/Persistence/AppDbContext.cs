using Microsoft.EntityFrameworkCore;
using SudokuVS.Game.Models;
using SudokuVS.Game.Models.Users;

namespace SudokuVS.Game.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserIdentityEntity> Users { get; set; }
    public DbSet<SudokuGameEntity> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SudokuGameEntity>().OwnsOne(g => g.Options);
        modelBuilder.Entity<SudokuGameEntity>().OwnsOne(g => g.Player1);
        modelBuilder.Entity<SudokuGameEntity>().OwnsOne(g => g.Player2);
    }
}
