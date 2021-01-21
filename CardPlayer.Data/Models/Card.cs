using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Data.Models
{
    public class Card
    {
        public string Name { get; }
        public string ShortName { get; }
        public Rank CardRank { get; }
        public Suit CardSuit { get; }
        public bool Suited { get; }
        public Card(Rank rank, Suit suit)
        {
            Suited = true;
            CardRank = rank;
            CardSuit = suit;
            Name = $"{CardRank.Name} of {CardSuit.Name}";
            ShortName = $"{CardRank.ShortName}{CardSuit.ShortName}";
        }
        public Card(string name, string shortName)
        {
            Suited = false;
            Name = name;
            ShortName = shortName;
            CardRank = null;
            CardSuit = null;
        }

        public Card()
        {

        }

        public override string ToString()
        {
            return ShortName;
        }
    }
}
