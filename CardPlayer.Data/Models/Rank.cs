using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardPlayer.Data.Models
{
    public class Rank
    {
        public string Name { get; set; }
        public char ShortName { get; set; }
        
        [JsonConstructor]
        public Rank(string name, char shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public Rank()
        {
        }
    }
}
