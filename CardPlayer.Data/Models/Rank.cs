using System;
using System.Collections.Generic;
using System.Text;

namespace CardPlayer.Data.Models
{
    public class Rank
    {
        public string Name { get; set; }
        public char ShortName { get; set; }
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
