using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Data.Models
{
    public class Deck
    {
        private readonly Random _rng;
        public DeckType TypeOfDeck { get; }
        public List<Card> Cards { get; set; }

        public Deck(StandardDecks knownType = StandardDecks.Traditional)
        {
            _rng = new Random();
            TypeOfDeck = DeckType.MakeDeckType(knownType);
            Cards = new List<Card>();
            foreach (var suit in TypeOfDeck.OrderedSuits)
                foreach (var rank in TypeOfDeck.OrderedRanks)
                    Cards.Add(new Card(rank, suit));
            foreach (var card in TypeOfDeck.OtherCards)
                Cards.Add(card);
        }

        public Deck(Deck source)
        {
            TypeOfDeck = source.TypeOfDeck;
            Cards = new List<Card>();
            Cards.AddRange(source.Cards);
        }

        public Deck Shuffle()
        {
            Deck ShuffledDeck = new Deck(this)
            {
                Cards = ( 
                    from card in this.Cards
                    orderby _rng.Next()
                    select card
                ).ToList()
            };
            return ShuffledDeck;
        }

    }
}
