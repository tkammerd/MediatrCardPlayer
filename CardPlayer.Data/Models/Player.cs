using System;
using System.Collections.Generic;
using System.Text;

namespace CardPlayer.Data.Models
{
    public class Player
    {
        public string Name { get; set; } = "";
        public Hand PlayerHand { get; set; } = new Hand();
    }
}

