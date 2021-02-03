using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardPlayer.Data.Models
{
    public class Hand
    {
        public List<Card> Cards { get; set; } = new List<Card>();

        [JsonConstructor]
        public Hand(List<Card> cards)
        {
            Cards = cards;
        }

        public Hand() { }
    }
}
