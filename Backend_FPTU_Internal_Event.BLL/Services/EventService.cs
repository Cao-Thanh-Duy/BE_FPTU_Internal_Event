using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly ISpeakerRepository _speakerRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly IUserRepository _userRepository;

        public EventService(
            IEventRepository eventRepository,
            IVenueRepository venueRepository,
            ISpeakerRepository speakerRepository,
            ISlotRepository slotRepository,
            IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _speakerRepository = speakerRepository;
            _slotRepository = slotRepository;
            _userRepository = userRepository;
        }

        private EventDTO MapToDTO(Event eventEntity)
        {
            return new EventDTO
            {
                EventId = eventEntity.EventId,
                EventName = eventEntity.EventName,
                EventDescription = eventEntity.EventDescription,
                EventDay = eventEntity.EventDate,
                MaxTickerCount = eventEntity.MaxTicketCount,
                CurrentTickerCount = eventEntity.CurrentTicketCount,
                Status = eventEntity.Status,
                VenueName = eventEntity.Venue?.VenueName ?? string.Empty,
                LocationDetails = eventEntity.Venue?.LocationDetails ?? string.Empty
            };
        }

        public EventDTO? CreateEvent(CreateEventRequest request, int organizerId)
        {
            throw new NotImplementedException();
        }

        public List<EventDTO> GetAllEvents()
        {
            throw new NotImplementedException();
        }

        public EventDTO? GetEventById(int eventId)
        {
            throw new NotImplementedException();
        }
    }
}
