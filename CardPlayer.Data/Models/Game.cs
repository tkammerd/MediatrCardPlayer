using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CardPlayer.Data.Models
{
    public class Game
    {
        public int Id { get; set; }
        public StandardDecks StandardDeckType { get; set; }
        [Range(2, 10)]
        public int HandSize { get; set; }
        [Range(2, 8)]
        public int MaximumPlayers { get; set; }
        public Deck StartDeck { get; set; }
        public Deck ShuffledDeck { get; set; }
        public List<Player> Players { get; set; }
        public Task TaskId { get; set; }
        public Game(StandardDecks standardDeckType = StandardDecks.Traditional)
        {
            HandSize = 0;
            MaximumPlayers = 0;
            StandardDeckType = standardDeckType;
            StartDeck = new Deck(standardDeckType);
            ShuffledDeck = StartDeck.Shuffle();
            Players = new List<Player>();
        }
    }
}
