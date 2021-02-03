using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Web.Models
{
    public class AddedPlayerViewModel
    {
        public bool PlayerAdded { get; set; }
        public bool PlayerIsNew { get; set; }
        public string PlayerName { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public bool GameIsFull => CurrentPlayers >= MaxPlayers;
    }
}
