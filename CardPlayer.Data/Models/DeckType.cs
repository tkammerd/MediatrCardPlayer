using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardPlayer.Data.Models
{
    public class DeckType
    {
        public string Name { get; set; }
        public List<Suit> OrderedSuits { get; set; }
        public List<Rank> OrderedRanks { get; set; }
        public List<Card> OtherCards { get; set; }

        [JsonConstructor]
        public DeckType(string name, List<Suit> orderedSuits, 
            List<Rank> orderedRanks, List<Card> otherCards)
        {
            Name = name;
            OrderedSuits = orderedSuits;
            OrderedRanks = orderedRanks;
            OtherCards = otherCards;
        }
    
        public DeckType() { }

        public static DeckType MakeDeckType ( StandardDecks standardDeck)
        {
            DeckType Result = new DeckType
            {
                OtherCards = standardDeck switch
                {
                    StandardDecks.Poker => new List<Card>
                    {
                        new Card("Joker", "JK"), new Card("Joker", "JK")
                    },
                    var x when x == StandardDecks.Traditional || x== StandardDecks.Euchre ||
                        x== StandardDecks.EightDeckBlackjackShoe => new List<Card>(),
                    StandardDecks.Uno => new List<Card>
                    {
                        new Card("Wild",        "WC"), new Card("Wild",        "WC"),
                        new Card("Wild",        "WC"), new Card("Wild",        "WC"),
                        new Card("Wild Draw 4", "W4"), new Card("Wild Draw 4", "W4"),
                        new Card("Wild Draw 4", "W4"), new Card("Wild Draw 4", "W4")
                    },
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(standardDeck), standardDeck, "Invalid Deck Type"),
                },
                Name = standardDeck switch
                {
                    StandardDecks.Traditional => nameof(StandardDecks.Traditional),
                    StandardDecks.Poker => nameof(StandardDecks.Poker),
                    StandardDecks.Euchre => nameof(StandardDecks.Euchre),
                    StandardDecks.EightDeckBlackjackShoe => nameof(StandardDecks.EightDeckBlackjackShoe),
                    StandardDecks.Uno => nameof(StandardDecks.Uno),
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(standardDeck), standardDeck, "Invalid Deck Type"),
                },
                OrderedSuits = standardDeck switch
                {
                    var x when x == StandardDecks.Traditional || x == StandardDecks.Euchre ||
                        x == StandardDecks.EightDeckBlackjackShoe => new List<Suit>
                    {
                        new Suit("Clubs", 'C'),
                        new Suit("Diamonds", 'D'),
                        new Suit("Hearts", 'H'),
                        new Suit("Spades", 'S'),
                    },
                    StandardDecks.EightDeckBlackjackShoe => new List<Suit>
                    {
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S')
                    },
                    StandardDecks.Uno => new List<Suit>
                    {
                        new Suit("Reds",    'R'),
                        new Suit("Yellows", 'Y'),
                        new Suit("Greens",  'G'),
                        new Suit("Blues",   'B'),
                    },
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(standardDeck), standardDeck, "Invalid Deck Type"),
                },
                OrderedRanks = standardDeck switch
                {
                    var x when x == StandardDecks.Traditional || x == StandardDecks.Euchre ||
                        x == StandardDecks.EightDeckBlackjackShoe => new List<Rank>
                    {
                        new Rank("Two", '2'),
                        new Rank("Three", '3'),
                        new Rank("Four", '4'),
                        new Rank("Five", '5'),
                        new Rank("Six", '6'),
                        new Rank("Seven", '7'),
                        new Rank("Eight", '8'),
                        new Rank("Nine", '9'),
                        new Rank("Ten", 'T'),
                        new Rank("Jack", 'J'),
                        new Rank("Queen", 'Q'),
                        new Rank("King", 'K'),
                        new Rank("Ace", 'A'),
                    },
                    StandardDecks.Euchre => new List<Rank>
                    {
                        new Rank("Nine", '9'),
                        new Rank("Ten", 'T'),
                        new Rank("Jack", 'J'),
                        new Rank("Queen", 'Q'),
                        new Rank("King", 'K'),
                        new Rank("Ace", 'A'),
                    },
                    StandardDecks.Uno => new List<Rank>
                    {
                        new Rank("Zero",     '0'),
                        new Rank("One",      '1'), new Rank("One",      '1'),
                        new Rank("Two",      '2'), new Rank("Two",      '2'),
                        new Rank("Three",    '3'), new Rank("Three",    '3'),
                        new Rank("Four",     '4'), new Rank("Four",     '4'),
                        new Rank("Five",     '5'), new Rank("Five",     '5'),
                        new Rank("Six",      '6'), new Rank("Six",      '6'),
                        new Rank("Seven",    '7'), new Rank("Seven",    '7'),
                        new Rank("Eight",    '8'), new Rank("Eight",    '8'),
                        new Rank("Nine",     '9'), new Rank("Nine",     '9'),
                        new Rank("Draw Two", 'D'), new Rank("Draw Two", 'D'),
                        new Rank("Skip",     'S'), new Rank("Skip",     'S'),
                        new Rank("Reverse",  'R'), new Rank("Reverse",  'R'),
                    },
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(standardDeck), standardDeck, "Invalid Deck Type"),
                }
            };
            return Result;
        }
    }
}
