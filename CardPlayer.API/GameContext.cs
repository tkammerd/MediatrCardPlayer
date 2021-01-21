using CardPlayer.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CardPlayer.API
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        public DbSet<Game> Game { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasData(
                new Game() { HandSize = 5, MaximumPlayers = 2 }
            );
        }
    }
}