using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Web.Models
{
    public class TextViewModel
    {
        public string Text { get; set; } = "Message text has not been set";
        public string CssStyle { get; set; } = "";
        public string Title { get; set; } = "Message";
        public string BackLinkLabel { get; set; } = "Back to Foyer";
        public string BackLink { get; set; } = "Index";
    }
}
