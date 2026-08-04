using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Common.InterFaces
{
    public interface IApplicationDbContext
    {
        DbSet<Venue> Venues { get; }
        DbSet<Hall> Halls { get; }
        DbSet<Service> Services { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingService> BookingServices { get; }
        DbSet<Event> Events { get; }
        DbSet<TicketTier> Tickets { get; }
        DbSet<Review> Reviews { get; }
        DbSet<User> Users { get; }
        DbSet<UserTicket> UserTickets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
