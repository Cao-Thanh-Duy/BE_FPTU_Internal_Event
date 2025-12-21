using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Microsoft.Extensions.Logging;
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

            // Validate MaxTicketCount <= MaxSeat của Venue
            if (request.MaxTicketCount > venue.MaxSeat)
            {
                throw new InvalidOperationException($"MaxTicketCount ({request.MaxTicketCount}) cannot exceed Venue's MaxSeat ({venue.MaxSeat})");
            }

            // Validate EventDate is not in the past
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (request.EventDate < today)
            {
                throw new InvalidOperationException($"EventDate ({request.EventDate}) cannot be in the past. Event must be scheduled for today or a future date.");
            }


            // Validate User (Organizer) exists
            var organizer = _userRepository.GetUserById(organizerId);
            if (organizer == null)
            {
                throw new KeyNotFoundException($"User with ID {organizerId} not found");
            }

            // Validate Slots are not in the past (if event is today)
            if (request.EventDate == today && request.SlotIds != null && request.SlotIds.Any())
            {
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                var pastSlots = new List<string>();

                foreach (var slotId in request.SlotIds.Distinct())
                {
                    var slot = _slotRepository.GetSlotById(slotId);
                    if (slot == null)
                    {
                        throw new KeyNotFoundException($"Slot with ID {slotId} not found");
                    }

                    // Check if slot's start time has already passed
                    if (slot.StartTime <= currentTime)
                    {
                        pastSlots.Add($"{slot.SlotName} (starts at {slot.StartTime} - end at {slot.EndtTime} )");
                    }
                }

                if (pastSlots.Any())
                {
                    throw new InvalidOperationException(
                        $"Cannot schedule event for today with slots that have already started or passed. Current time: {currentTime:HH:mm}. Past slots: {string.Join(", ", pastSlots)}");
                }
            }

            // Validate Slots are not occupied at this venue on this date
            if (request.SlotIds != null && request.SlotIds.Any())
            {
                var occupiedSlots = new List<int>();

                foreach (var slotId in request.SlotIds.Distinct())
                {
                    // Check if slot exists
                    var slot = _slotRepository.GetSlotById(slotId);
                    if (slot == null)
                    {
                        throw new KeyNotFoundException($"Slot with ID {slotId} not found");
                    }

                    // Check if slot is already occupied
                    if (_eventRepository.IsSlotOccupiedExcludeRejected(request.VenueId, request.EventDate, slotId))
                    {
                        occupiedSlots.Add(slotId);
                    }
                }

                if (occupiedSlots.Any())
                {
                    var slotNames = occupiedSlots
                        .Select(id => _slotRepository.GetSlotById(id)?.SlotName ?? $"Slot {id}")
                        .ToList();

                    throw new InvalidOperationException(
                        $"The following slots are already occupied at venue '{venue.VenueName}' on {request.EventDate}: {string.Join(", ", slotNames)}");
                }
            }

            // Validate Speakers are not occupied in the same slots on this date
            if (request.SpeakerIds != null && request.SpeakerIds.Any() &&
                request.SlotIds != null && request.SlotIds.Any())
            {
                var occupiedSpeakerSlots = new Dictionary<int, List<int>>(); // SpeakerId -> List of occupied SlotIds

                foreach (var speakerId in request.SpeakerIds.Distinct())
                {
                    // Check if speaker exists
                    var speaker = _speakerRepository.GetSpeakerById(speakerId);
                    if (speaker == null)
                    {
                        throw new KeyNotFoundException($"Speaker with ID {speakerId} not found");
                    }

                    var occupiedSlots = new List<int>();

                    // Check each slot for this speaker
                    foreach (var slotId in request.SlotIds.Distinct())
                    {
                        if (_eventRepository.IsSpeakerOccupiedInSlot(speakerId, request.EventDate, slotId))
                        {
                            occupiedSlots.Add(slotId);
                        }
                    }

                    if (occupiedSlots.Any())
                    {
                        occupiedSpeakerSlots[speakerId] = occupiedSlots;
                    }
                }

                if (occupiedSpeakerSlots.Any())
                {
                    var errorMessages = occupiedSpeakerSlots.Select(kvp =>
                    {
                        var speakerName = _speakerRepository.GetSpeakerById(kvp.Key)?.SpeakerName ?? $"Speaker {kvp.Key}";
                        var slotNames = kvp.Value.Select(slotId =>
                            _slotRepository.GetSlotById(slotId)?.SlotName ?? $"Slot {slotId}");
                        return $"{speakerName} is already occupied in: {string.Join(", ", slotNames)}";
                    });

                    throw new InvalidOperationException(
                        $"The following speakers have conflicts on {request.EventDate}:\n{string.Join("\n", errorMessages)}");
                }
            }

            // Validate Staff are not occupied in the same slots on this date
            if (request.StaffIds != null && request.StaffIds.Any() &&
                request.SlotIds != null && request.SlotIds.Any())
            {
                var occupiedStaffSlots = new Dictionary<int, List<int>>(); // StaffId -> List of occupied SlotIds

                foreach (var staffId in request.StaffIds.Distinct())
                {
                    // Check if staff exists
                    var staff = _userRepository.GetUserById(staffId);
                    if (staff == null)
                    {
                        throw new KeyNotFoundException($"Staff with ID {staffId} not found");
                    }

                    var occupiedSlots = new List<int>();

                    // Check each slot for this staff
                    foreach (var slotId in request.SlotIds.Distinct())
                    {
                        if (_eventRepository.IsStaffOccupiedInSlot(staffId, request.EventDate, slotId))
                        {
                            occupiedSlots.Add(slotId);
                        }
                    }

                    if (occupiedSlots.Any())
                    {
                        occupiedStaffSlots[staffId] = occupiedSlots;
                    }
                }

                if (occupiedStaffSlots.Any())
                {
                    var errorMessages = occupiedStaffSlots.Select(kvp =>
                    {
                        var staffName = _userRepository.GetUserById(kvp.Key)?.UserName ?? $"Staff {kvp.Key}";
                        var slotNames = kvp.Value.Select(slotId =>
                            _slotRepository.GetSlotById(slotId)?.SlotName ?? $"Slot {slotId}");
                        return $"{staffName} is already occupied in: {string.Join(", ", slotNames)}";
                    });

                    throw new InvalidOperationException(
                        $"The following staff members have conflicts on {request.EventDate}:\n{string.Join("\n", errorMessages)}");
                }
            }

            // Create Event
            var newEvent = new Event
            {
                EventName = request.EventName,
                EventDescription = request.EventDescription,
                EventDate = request.EventDate,
                MaxTicketCount = request.MaxTicketCount,
                CurrentTicketCount = request.MaxTicketCount,
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

            // NEW: Get Staff
            List<StaffEventDTO> staffEventDTOs = new();
            var staffEvents = _eventRepository.GetAllStaffEvents(eventEntity.EventId);
            foreach (var staffEvent in staffEvents)
            {
                if (staffEvent.User != null)
                {
                    StaffEventDTO staffEventDTO = new()
                    {
                        UserId = staffEvent.UserId,
                        UserName = staffEvent.User.UserName,
                        Email = staffEvent.User.Email,
                        RoleName = staffEvent.User.Role?.RoleName ?? "Unknown"
                    };
                    staffEventDTOs.Add(staffEventDTO);
                }
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
                SpeakerEvent = speakerEventDTOs,
                StaffEvent = staffEventDTOs

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
            if (eventReject == null)
            {
                throw new KeyNotFoundException($"Event Id {enentId} do not exist");
            }

            // Set status to Reject
            eventReject.Status = "Reject";

           
            //  Remove all relationships
           

            // 1. Remove all SpeakerEvents (free speakers)
            var speakerEvents = _eventRepository.GetAllSpeakerEvents(enentId);
            foreach (var speakerEvent in speakerEvents)
            {
                _eventRepository.RemoveSpeakerEvent(speakerEvent);
            }

            // 2. Remove all EventSchedules (free slots)
            var eventSchedules = _eventRepository.GetAllEventSchedules(enentId);
            foreach (var eventSchedule in eventSchedules)
            {
                _eventRepository.RemoveEventSchedule(eventSchedule);
            }

            // 3. Remove all StaffEvents (free staff)
            var staffEvents = _eventRepository.GetAllStaffEvents(enentId);
            foreach (var staffEvent in staffEvents)
            {
                _eventRepository.RemoveStaffEvent(staffEvent);
            }

            // Save all changes
            _eventRepository.SaveChanges();

            return true;
        }

        public List<EventDTO> GetEventsByOrganizerId(int organizerId)
        {
            var events = _eventRepository.GetEventsByOrganizerId(organizerId);
            return events.Select(e => MapToDTO(e)).ToList();
        }

        public EventDTO? UpdateEvent(int eventId, CreateUpdateEventRequest request)
        {
            // Get existing event
            var existingEvent = _eventRepository.GetEventById(eventId);
            if (existingEvent == null)
            {
                throw new KeyNotFoundException($"Event with ID {eventId} not found");
            }

            // Check if event can be updated (must be at least 2 days before event starts)
            var today = DateOnly.FromDateTime(DateTime.Today);
            var eventDate = request.EventDate ?? existingEvent.EventDate;
            var daysUntilEvent = eventDate.DayNumber - today.DayNumber;

            if (daysUntilEvent < 2)
            {
                throw new InvalidOperationException($"Cannot update event. Updates must be made at least 2 days before the event date. Event date: {eventDate}, Current date: {today}");
            }

            // Validate EventDate if provided
            if (request.EventDate.HasValue)
            {
                if (request.EventDate.Value < today)
                {
                    throw new InvalidOperationException($"EventDate ({request.EventDate.Value}) cannot be in the past. Event must be scheduled for today or a future date.");
                }

                // Validate Slots are not in the past (if event is today)
                if (request.EventDate.Value == today && request.SlotIds != null && request.SlotIds.Any())
                {
                    var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    var pastSlots = new List<string>();

                    foreach (var slotId in request.SlotIds.Distinct())
                    {
                        var slot = _slotRepository.GetSlotById(slotId);
                        if (slot == null)
                        {
                            throw new KeyNotFoundException($"Slot with ID {slotId} not found");
                        }

                        if (slot.StartTime <= currentTime)
                        {
                            pastSlots.Add($"{slot.SlotName} (starts at {slot.StartTime} - ends at {slot.EndtTime})");
                        }
                    }

                    if (pastSlots.Any())
                    {
                        throw new InvalidOperationException(
                            $"Cannot schedule event for today with slots that have already started or passed. Current time: {currentTime:HH:mm}. Past slots: {string.Join(", ", pastSlots)}");
                    }
                }
            }

            // Get current venue for comparison
            var currentVenue = _venueRepository.GetVenueById(existingEvent.VenueId);
            Venue? newVenue = null;

            // Validate Venue if provided
            if (request.VenueId.HasValue)
            {
                newVenue = _venueRepository.GetVenueById(request.VenueId.Value);
                if (newVenue == null)
                {
                    throw new KeyNotFoundException($"Venue with ID {request.VenueId.Value} not found");
                }

                // VALIDATION: MaxSeat của venue mới phải >= MaxSeat của venue cũ
                if (newVenue.MaxSeat < currentVenue.MaxSeat)
                {
                    throw new InvalidOperationException(
                        $"Cannot change to new venue. New venue's MaxSeat ({newVenue.MaxSeat}) must be greater than or equal to current venue's MaxSeat ({currentVenue.MaxSeat}) to accommodate existing tickets.");
                }

                // Validate MaxTicketCount <= MaxSeat của Venue mới
                var maxTicketCount = request.MaxTicketCount ?? existingEvent.MaxTicketCount;
                if (maxTicketCount > newVenue.MaxSeat)
                {
                    throw new InvalidOperationException($"MaxTicketCount ({maxTicketCount}) cannot exceed new Venue's MaxSeat ({newVenue.MaxSeat})");
                }
            }

            // Validate MaxTicketCount if provided (without venue change)
            if (request.MaxTicketCount.HasValue)
            {
                var venueToCheck = newVenue ?? currentVenue;

                if (request.MaxTicketCount.Value > venueToCheck.MaxSeat)
                {
                    throw new InvalidOperationException($"MaxTicketCount ({request.MaxTicketCount.Value}) cannot exceed Venue's MaxSeat ({venueToCheck.MaxSeat})");
                }

                //////////////////////////////////////////////
                // VALIDATION 1: MaxTicketCount mới phải >= MaxTicketCount cũ
                if (request.MaxTicketCount.Value < existingEvent.MaxTicketCount)
                {
                    throw new InvalidOperationException(
                        $"Cannot reduce MaxTicketCount from {existingEvent.MaxTicketCount} to {request.MaxTicketCount.Value}. MaxTicketCount can only be increased or kept the same.");
                }

                // VALIDATION 2: Không cho giảm MaxTicketCount xuống thấp hơn số ticket đã bán (redundant check nhưng để rõ ràng)
                var ticketsSold = existingEvent.MaxTicketCount - existingEvent.CurrentTicketCount;
                if (request.MaxTicketCount.Value < ticketsSold)
                {
                    throw new InvalidOperationException($"Cannot reduce MaxTicketCount to {request.MaxTicketCount.Value}. Already sold {ticketsSold} tickets.");
                }








            }

            // Validate Slots if provided
            if (request.SlotIds != null && request.SlotIds.Any())
            {
                var venueId = request.VenueId ?? existingEvent.VenueId;
                var occupiedSlots = new List<int>();

                foreach (var slotId in request.SlotIds.Distinct())
                {
                    var slot = _slotRepository.GetSlotById(slotId);
                    if (slot == null)
                    {
                        throw new KeyNotFoundException($"Slot with ID {slotId} not found");
                    }

                    // Check if slot is already occupied by OTHER events (exclude current event)
                    var isOccupied = _eventRepository.IsSlotOccupiedExcludeEvent(venueId, eventDate, slotId, eventId);
                    if (isOccupied)
                    {
                        occupiedSlots.Add(slotId);
                    }
                }

                if (occupiedSlots.Any())
                {
                    var venue = _venueRepository.GetVenueById(venueId);
                    var slotNames = occupiedSlots
                        .Select(id => _slotRepository.GetSlotById(id)?.SlotName ?? $"Slot {id}")
                        .ToList();

                    throw new InvalidOperationException(
                        $"The following slots are already occupied at venue '{venue.VenueName}' on {eventDate}: {string.Join(", ", slotNames)}");
                }
            }

            // Validate Speakers if provided
            if (request.SpeakerIds != null && request.SpeakerIds.Any())
            {
                var slotIds = request.SlotIds ??
                             _eventRepository.GetAllEventSchedules(eventId).Select(es => es.SlotId).ToList();

                if (slotIds.Any())
                {
                    var occupiedSpeakerSlots = new Dictionary<int, List<int>>();

                    foreach (var speakerId in request.SpeakerIds.Distinct())
                    {
                        var speaker = _speakerRepository.GetSpeakerById(speakerId);
                        if (speaker == null)
                        {
                            throw new KeyNotFoundException($"Speaker with ID {speakerId} not found");
                        }

                        var occupiedSlots = new List<int>();

                        foreach (var slotId in slotIds.Distinct())
                        {
                            // Check if speaker is occupied, excluding current event
                            var isOccupied = _eventRepository.IsSpeakerOccupiedInSlotExcludeEvent(speakerId, eventDate, slotId, eventId);
                            if (isOccupied)
                            {
                                occupiedSlots.Add(slotId);
                            }
                        }

                        if (occupiedSlots.Any())
                        {
                            occupiedSpeakerSlots[speakerId] = occupiedSlots;
                        }
                    }

                    if (occupiedSpeakerSlots.Any())
                    {
                        var errorMessages = occupiedSpeakerSlots.Select(kvp =>
                        {
                            var speakerName = _speakerRepository.GetSpeakerById(kvp.Key)?.SpeakerName ?? $"Speaker {kvp.Key}";
                            var slotNames = kvp.Value.Select(slotId =>
                                _slotRepository.GetSlotById(slotId)?.SlotName ?? $"Slot {slotId}");
                            return $"{speakerName} is already occupied in: {string.Join(", ", slotNames)}";
                        });

                        throw new InvalidOperationException(
                            $"The following speakers have conflicts on {eventDate}:\n{string.Join("\n", errorMessages)}");
                    }
                }
            }

            // Validate Staff if provided
            if (request.StaffIds != null && request.StaffIds.Any())
            {
                var slotIds = request.SlotIds ??
                             _eventRepository.GetAllEventSchedules(eventId).Select(es => es.SlotId).ToList();

                if (slotIds.Any())
                {
                    var occupiedStaffSlots = new Dictionary<int, List<int>>();

                    foreach (var staffId in request.StaffIds.Distinct())
                    {
                        var staff = _userRepository.GetUserById(staffId);
                        if (staff == null)
                        {
                            throw new KeyNotFoundException($"Staff with ID {staffId} not found");
                        }

                        var occupiedSlots = new List<int>();

                        foreach (var slotId in slotIds.Distinct())
                        {
                            // Check if staff is occupied, excluding current event
                            var isOccupied = _eventRepository.IsStaffOccupiedInSlotExcludeEvent(staffId, eventDate, slotId, eventId);
                            if (isOccupied)
                            {
                                occupiedSlots.Add(slotId);
                            }
                        }

                        if (occupiedSlots.Any())
                        {
                            occupiedStaffSlots[staffId] = occupiedSlots;
                        }
                    }

                    if (occupiedStaffSlots.Any())
                    {
                        var errorMessages = occupiedStaffSlots.Select(kvp =>
                        {
                            var staffName = _userRepository.GetUserById(kvp.Key)?.UserName ?? $"Staff {kvp.Key}";
                            var slotNames = kvp.Value.Select(slotId =>
                                _slotRepository.GetSlotById(slotId)?.SlotName ?? $"Slot {slotId}");
                            return $"{staffName} is already occupied in: {string.Join(", ", slotNames)}";
                        });

                        throw new InvalidOperationException(
                            $"The following staff members have conflicts on {eventDate}:\n{string.Join("\n", errorMessages)}");
                    }
                }
            }

            // Update basic event properties
            existingEvent.EventName = request.EventName;
            existingEvent.EventDescription = request.EventDescription;

            if (request.EventDate.HasValue)
                existingEvent.EventDate = request.EventDate.Value;

            // Update VenueId if provided and validated
            if (request.VenueId.HasValue)
                existingEvent.VenueId = request.VenueId.Value;

            if (request.MaxTicketCount.HasValue)
            {
                // Adjust CurrentTicketCount if MaxTicketCount changes
                var difference = request.MaxTicketCount.Value - existingEvent.MaxTicketCount;
                existingEvent.MaxTicketCount = request.MaxTicketCount.Value;
                existingEvent.CurrentTicketCount += difference;
            }

            // Set status back to Pending for re-approval
            existingEvent.Status = "Pending";

            // Update Speakers if provided
            if (request.SpeakerIds != null)
            {
                // Remove existing speakers
                var existingSpeakers = _eventRepository.GetAllSpeakerEvents(eventId);
                foreach (var se in existingSpeakers)
                {
                    _eventRepository.RemoveSpeakerEvent(se);
                }

                // Add new speakers
                foreach (var speakerId in request.SpeakerIds.Distinct())
                {
                    var speaker = _speakerRepository.GetSpeakerById(speakerId);
                    if (speaker != null)
                    {
                        _eventRepository.AddSpeakerEvent(new SpeakerEvent
                        {
                            EventId = eventId,
                            SpeakerId = speakerId
                        });
                    }
                }
            }

            // Update Slots if provided
            if (request.SlotIds != null)
            {
                // Remove existing slots
                var existingSlots = _eventRepository.GetAllEventSchedules(eventId);
                foreach (var es in existingSlots)
                {
                    _eventRepository.RemoveEventSchedule(es);
                }

                // Add new slots
                foreach (var slotId in request.SlotIds.Distinct())
                {
                    var slot = _slotRepository.GetSlotById(slotId);
                    if (slot != null)
                    {
                        _eventRepository.AddEventSchedule(new EventSchedule
                        {
                            EventId = eventId,
                            SlotId = slotId
                        });
                    }
                }
            }

            // Update Staff if provided
            if (request.StaffIds != null)
            {
                // Remove existing staff
                var existingStaff = _eventRepository.GetAllStaffEvents(eventId);
                foreach (var se in existingStaff)
                {
                    _eventRepository.RemoveStaffEvent(se);
                }

                // Add new staff
                foreach (var staffId in request.StaffIds.Distinct())
                {
                    var staff = _userRepository.GetUserById(staffId);
                    if (staff != null)
                    {
                        _eventRepository.AddStaffEvent(new StaffEvent
                        {
                            EventId = eventId,
                            UserId = staffId
                        });
                    }
                }
            }

            _eventRepository.SaveChanges();

            // Reload and return updated event
            var updatedEvent = _eventRepository.GetEventById(eventId);
            return updatedEvent == null ? null : MapToDTO(updatedEvent);
        }

        public List<EventDTO> GetEventsByStaffId(int staffId)
        {
            var events = _eventRepository.GetEventsByStaffId(staffId);
            return events.Select(e => MapToDTO(e)).ToList();
        }
    }
}