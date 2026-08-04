using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Tickets.Dtos
{
    public class PurchaseTicketDto:IRequest<List<ResponseUserTicketDto>>
    {
        public Guid TicketTypeId { get; set; } 
        public int Quantity { get; set; } = 1;
    }
}
