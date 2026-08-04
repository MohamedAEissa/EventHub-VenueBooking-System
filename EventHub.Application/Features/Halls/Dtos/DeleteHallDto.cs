using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Dtos
{
    public class DeleteHallDto : IRequest<bool>
    {
        public Guid VenueId { get; set; }
        public Guid HallId { get; set; }

        public DeleteHallDto(Guid venueId, Guid hallId)
        {
            VenueId = venueId;
            HallId = hallId;
        }
    }
}
