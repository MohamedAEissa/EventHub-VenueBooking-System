using AutoMapper;
using EventHub.Application.Features.Auth.Dtos;
using EventHub.Application.Features.Bookings.Dtos;
using EventHub.Application.Features.Events.Dtos;
using EventHub.Application.Features.Halls.Dtos;
using EventHub.Application.Features.Reviews.Dtos;
using EventHub.Application.Features.Services.Dtos;
using EventHub.Application.Features.Tickets.Dtos;
using EventHub.Application.Features.Venues.Dtos;
using EventHub.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Common.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            #region Mapping Auth
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Token, opt => opt.Ignore());

            #endregion

            #region Mapping Venues/Halls
            CreateMap<CreateHallDto, Hall>();

            CreateMap<Hall, ResponseHallDto>();

            CreateMap<CreateVenueDto,Venue>();

            CreateMap<Venue,ResponseVenueDto>()
                .ForMember(dest=>dest.OwnerName,opt=>opt.MapFrom(src=>src.Owner.FullName));
            #endregion

            #region Mapping Services & Bookings & Reviews
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest=>dest.BookingServices,opt=>opt.Ignore());

            CreateMap<Booking, ResponseBookingDto>()
                .ForMember(x => x.HallName, opt => opt.MapFrom(src => src.Hall.Name))
                .ForMember(x => x.VenueName, opt => opt.MapFrom(src => src.Hall.Venue.Name))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(x => x.Services, opt => opt.MapFrom(src => src.BookingServices));

            
            
            CreateMap<BookingService,ResponseBookingServicesDto >()
                .ForMember(dest=>dest.ServiceName,opt=>opt.MapFrom(src=>src.Service.Name));

            CreateMap<CreateReviewDto, Review>();
            CreateMap<Review, ResponseReviewDto>()
                .ForMember(dest=>dest.FullName,opt=>opt.MapFrom(src=>src.Client.FullName));

            CreateMap<CreateServiceDto,Service>();
            CreateMap<Service,ResponseServiceDto>();



            #endregion

            #region Event/Ticket

            CreateMap<CreateEventDto, Event>();
            CreateMap<Event, ResponseEventDto>()
                 .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Hall.Name))
                 .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Hall.Venue.Name))
                 .ForMember(dest => dest.TicketTypes, opt => opt.MapFrom(src => src.Tickets));

            CreateMap<TicketTier, ResponseTicketTypeDto>();
            CreateMap<CreateTicketTypeSubDto, TicketTier>()
                .ForMember(dest=>dest.AvailableQuantity,opt=>opt.MapFrom(src=>src.TotalQuantity));

            CreateMap<UserTicket, ResponseUserTicketDto>()
                .ForMember(dest=>dest.EventTitle,opt=>opt.MapFrom(src=>src.Ticket.Event.Title))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Ticket.Event.EventDate))
                .ForMember(dest => dest.TicketType, opt => opt.MapFrom(src => src.Ticket.Type))
                .ForMember(dest => dest.PricePaid, opt => opt.MapFrom(src => src.Ticket.Price));

            #endregion
        }
    }
}
