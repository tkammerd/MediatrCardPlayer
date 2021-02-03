using CardPlayer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Web.Models
{
    public class CardTableViewModel
    {
        public string Title { get; set; }
        public Deck UndealtCards { get; set; }
        public List<Hand> Hands { get; set; }
        public List<string> PlayerNames { get; set; }

        public CardTableViewModel(string title = "", Deck undealtCards = null,
            List<Hand> hands = null, List<string> playerNames = null)
        {
            Title = title;
            UndealtCards = undealtCards ?? new Deck();
            Hands = hands ?? new List<Hand>();
            PlayerNames = playerNames ?? new List<string>();
        }
    }
}