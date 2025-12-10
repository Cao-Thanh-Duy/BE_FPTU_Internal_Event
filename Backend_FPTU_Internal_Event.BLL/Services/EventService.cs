using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<EventDTO> GetAllEvents()
        {
            var events = _eventRepository.GetAllEvents();
            return events.Select(e => MapToDTO(e)).ToList();
        }

        public EventDTO? GetEventById(int eventId)
        {
            var eventEntity = _eventRepository.GetEventById(eventId);
            return eventEntity == null ? null : MapToDTO(eventEntity);
        }

        public EventDTO? CreateEvent(CreateEventRequest request, int organizerId)
        {
            // Validate Venue exists
            var venue = _venueRepository.GetVenueById(request.VenueId);
            if (venue == null)
            {
                throw new KeyNotFoundException($"Venue with ID {request.VenueId} not found");
            }

            // Validate User (Organizer) exists
            var organizer = _userRepository.GetUserById(organizerId);
            if (organizer == null)
            {
                throw new KeyNotFoundException($"User with ID {organizerId} not found");
            }

            // Create Event
            var newEvent = new Event
            {
                EventName = request.EventName,
                EventDescription = request.EventDescription,
                EventDate = request.EventDate,
                MaxTicketCount = request.MaxTicketCount,
                CurrentTicketCount = 0,
                Status = "Pending",
                UserId = organizerId,
                VenueId = request.VenueId
            };

            _eventRepository.AddEvent(newEvent);
            _eventRepository.SaveChanges();

            // Add Speakers (if provided)
            if (request.SpeakerIds != null && request.SpeakerIds.Any())
            {
                foreach (var speakerId in request.SpeakerIds.Distinct())
                {
                    var speaker = _speakerRepository.GetSpeakerById(speakerId);
                    if (speaker != null)
                    {
                        _eventRepository.AddSpeakerEvent(new SpeakerEvent
                        {
                            EventId = newEvent.EventId,
                            SpeakerId = speakerId
                        });
                    }
                }
            }

            // Add Slots (if provided)
            if (request.SlotIds != null && request.SlotIds.Any())
            {
                foreach (var slotId in request.SlotIds.Distinct())
                {
                    var slot = _slotRepository.GetSlotById(slotId);
                    if (slot != null)
                    {
                        _eventRepository.AddEventSchedule(new EventSchedule
                        {
                            EventId = newEvent.EventId,
                            SlotId = slotId
                        });
                    }
                }
            }

            // Add Staff (if provided)
            if (request.StaffIds != null && request.StaffIds.Any())
            {
                foreach (var staffId in request.StaffIds.Distinct())
                {
                    var staff = _userRepository.GetUserById(staffId);
                    if (staff != null)
                    {
                        _eventRepository.AddStaffEvent(new StaffEvent
                        {
                            EventId = newEvent.EventId,
                            UserId = staffId
                        });
                    }
                }
            }

            _eventRepository.SaveChanges();

            // Reload event with relationships
            var createdEvent = _eventRepository.GetEventById(newEvent.EventId);
            return createdEvent == null ? null : MapToDTO(createdEvent);
        }

        private EventDTO MapToDTO(Event eventEntity)
        {
            List<EventSlotDTO> slotEventDto = new();
            var slotEvent = _eventRepository.GetAllEventSchedules(eventEntity.EventId);
            foreach (var slot in slotEvent)
            {
                var slotdto = _slotRepository.GetSlotById(slot.SlotId);
                EventSlotDTO evslot = new() { StartTime = slotdto.StartTime, EndTime = slotdto.EndtTime, SlotName = slotdto.SlotName };
                slotEventDto.Add(evslot);
            }
            List<SpeakerEventDTO> speakerEventDTOs = new();
            var speakerEvent = _eventRepository.GetAllSpeakerEvents(eventEntity.EventId);
            foreach (var speaker in speakerEvent)
            {
                var spearkerdto = _speakerRepository.GetSpeakerById(speaker.SpeakerId);
                SpeakerEventDTO speakerEventDTO = new() { SpeakerDescription = spearkerdto.SpeakerDescription, SpeakerName = spearkerdto.SpeakerName };
                speakerEventDTOs.Add(speakerEventDTO);
            }
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
                LocationDetails = eventEntity.Venue?.LocationDetails ?? string.Empty,
                SlotEvent = slotEventDto,
                SpeakerEvent = speakerEventDTOs

            };
        }

        public bool ApproveEvent(int eventId)
        {
            var eventApprove = _eventRepository.GetEventById(eventId);

            if (eventApprove == null)
            {
             
                throw new KeyNotFoundException($"Event Id {eventId} do not exist");
            }
            else
            {
                var status = "Approve";
                eventApprove.Status = status;
                _eventRepository.SaveChanges();
            }
            return true;
        }

        public bool RejectEvent(int enentId)
        {
            var eventReject = _eventRepository.GetEventById(enentId);
            if(eventReject == null)
            {
                
                throw new KeyNotFoundException($"Event Id {enentId} do not exist");
        
            }
            else
            {
                var status = "Reject";
                eventReject.Status = status;
                _eventRepository.SaveChanges();
            }
            return true;
        }
    }
}