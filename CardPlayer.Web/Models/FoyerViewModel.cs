using CardPlayer.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Web.Models
{
    public class FoyerViewModel
    {
        [Range(1, int.MaxValue)]
        public int GameIdSelected { get; set; }
        public List<Game> Games { get; set; }
    }
}
