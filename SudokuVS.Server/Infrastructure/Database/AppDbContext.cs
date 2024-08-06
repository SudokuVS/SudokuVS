using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SudokuVS.Server.Infrastructure.Database.Models;
using SudokuVS.Server.Infrastructure.Database.Models.Game;

namespace SudokuVS.Server.Infrastructure.Database;

class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PlayerStateEntity> PlayerStates { get; private set; }
    public DbSet<SudokuGameEntity> Games { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SudokuGameEntity>().HasMany(e => e.Players).WithOne(s => s.Game).HasForeignKey(s => s.GameId).IsRequired();
        modelBuilder.Entity<AppUser>().HasOne<PlayerStateEntity>().WithOne(s => s.User).HasForeignKey<PlayerStateEntity>(s => s.UserName).IsRequired();
    }
}
