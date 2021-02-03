using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardPlayer.Data.Models
{
    public class Suit
    {
        public string Name { get; set; }
        public char ShortName { get; set; }

        [JsonConstructor]
        public Suit(string name, char shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public Suit()
        {
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Suit rhs = (Suit)obj;
            return (Name == rhs.Name) && (ShortName == rhs.ShortName);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ ShortName.GetHashCode();
        }
    }
}
