using CardPlayer.Web.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardPlayer.Web.Handlers
{
    public class MessageRequestHandler : IRequestHandler<MessageRequest, string>
    {
        public async Task<string> Handle(MessageRequest request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => "Default Response");
        }
    }

    public class MessageSendHandler : IRequestHandler<MessageSend, bool>
    {
        public string Message { get; set; }
        public async Task<bool> Handle(MessageSend message, CancellationToken cancellationToken)
        {
            Message = message.MessageText;
            return await Task.Run(() => true);
        }
    }
}
