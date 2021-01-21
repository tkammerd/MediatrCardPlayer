using System;
using CardPlayer.Data.Models;

public class GameContext: DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public DbSet<Game> Game { get; set; }
    public DbSet<Card> Card { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder Entity<Game>() HasData(
            new Game()
        )
    }
}
