using CardPlayer.Data.Models;
using CardPlayer.Web.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardPlayer.Web.Handlers
{
    public class DealCardHandler : IRequestHandler<DealCard, Card>
    {
        public Task<Card> Handle(DealCard request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
