using System;
using System.Collections.Generic;
using System.Text;

namespace CardPlayer.Data.Models
{
    public class DeckType
    {
        public string Name { get; set; }
        public List<Suit> OrderedSuits { get; set; }
        public List<Rank> OrderedRanks { get; set; }
        public List<Card> OtherCards { get; set; }
        public static DeckType MakeDeckType ( StandardDecks standardDeck)
        {
            DeckType Result = new DeckType();
            switch (standardDeck)
            {
                case StandardDecks.Poker:
                    Result.OtherCards = new List<Card>
                    {
                        new Card("Joker", "JK"), new Card("Joker", "JK")
                    };
                    break;
                case StandardDecks.Traditional:
                case StandardDecks.Euchre:
                case StandardDecks.EightDeckBlackjackShoe:
                    Result.OtherCards = new List<Card>();
                    break;
                case StandardDecks.Uno:
                    Result.OtherCards = new List<Card>
                    {
                        new Card("Wild",        "WC"), new Card("Wild",        "WC"),
                        new Card("Wild",        "WC"), new Card("Wild",        "WC"),
                        new Card("Wild Draw 4", "W4"), new Card("Wild Draw 4", "W4"),
                        new Card("Wild Draw 4", "W4"), new Card("Wild Draw 4", "W4")
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid Deck Type");
            }
            Result.Name = standardDeck switch
            {
                StandardDecks.Traditional => nameof(StandardDecks.Traditional),
                StandardDecks.Poker => nameof(StandardDecks.Poker),
                StandardDecks.Euchre => nameof(StandardDecks.Euchre),
                StandardDecks.EightDeckBlackjackShoe => nameof(StandardDecks.EightDeckBlackjackShoe),
                StandardDecks.Uno => nameof(StandardDecks.Uno),
                _ => throw new ArgumentOutOfRangeException("Invalid Deck Type"),
            };
            switch (standardDeck)
            {
                case StandardDecks.Traditional:
                case StandardDecks.Poker:
                case StandardDecks.Euchre:
                    Result.OrderedSuits = new List<Suit>
                    {
                        new Suit("Clubs", 'C'),
                        new Suit("Diamonds", 'D'),
                        new Suit("Hearts", 'H'),
                        new Suit("Spades", 'S'),
                    };
                    break;
                case StandardDecks.EightDeckBlackjackShoe:
                    Result.OrderedSuits = new List<Suit>
                    {
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S'),
                        new Suit("Clubs", 'C'), new Suit("Diamonds", 'D'), new Suit("Hearts", 'H'), new Suit("Spades", 'S')
                    };
                    break;
                case StandardDecks.Uno:
                    Result.OrderedSuits = new List<Suit>
                    {
                        new Suit("Reds",    'R'),
                        new Suit("Yellows", 'Y'),
                        new Suit("Greens",  'G'),
                        new Suit("Blues",   'B'),
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid Deck Type");
            }
            switch (standardDeck)
            {
                case StandardDecks.Traditional:
                case StandardDecks.Poker:
                case StandardDecks.EightDeckBlackjackShoe:
                    Result.OrderedRanks = new List<Rank>
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
                    };
                    break;
                case StandardDecks.Euchre:
                    Result.OrderedRanks = new List<Rank>
                    {
                        new Rank("Nine", '9'),
                        new Rank("Ten", 'T'),
                        new Rank("Jack", 'J'),
                        new Rank("Queen", 'Q'),
                        new Rank("King", 'K'),
                        new Rank("Ace", 'A'),
                    };
                    break;
                case StandardDecks.Uno:
                    Result.OrderedRanks = new List<Rank>
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
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid Deck Type");
            }
            return Result;
        }
    }
}
