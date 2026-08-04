using EventHub.Application.Features.Auth.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Reviews.Dtos
{
    public class CreateReviewDto: IRequest<ResponseReviewDto>
    {
        public Guid VenueId { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
    }
}
