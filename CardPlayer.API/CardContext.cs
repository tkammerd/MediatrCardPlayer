using CardPlayer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CardPlayer.API
{
    public class CardContext : DbContext
    {
        public CardContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        public DbSet<Card> Card { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>().HasData(
                new Card("","")
            ); ;
        }
    }
}