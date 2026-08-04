using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Reviews.Dtos
{
    public class ResponseReviewDto
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}
