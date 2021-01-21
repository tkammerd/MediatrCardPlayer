using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.Web.Requests
{
    public class MessageRequest : IRequest<string>
    {
    }

    public class MessageSend : IRequest<bool>
    {
        public string MessageText { get; set; }

        public MessageSend(string messageText)
        {
            MessageText = messageText;
        }
    }
}
