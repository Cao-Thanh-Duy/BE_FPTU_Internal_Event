using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Event> GetAllEvents()
        {
            return _context.Events
                .Include(e => e.User)
                .Include(e => e.Venue)
                .ToList();
        }

        public Event? GetEventById(int eventId)
        {
            return _context.Events
                .Include(e => e.User)
                .Include(e => e.Venue)
                .FirstOrDefault(e => e.EventId == eventId);
        }

        public void AddEvent(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
        }

        public void AddSpeakerEvent(SpeakerEvent speakerEvent)
        {
            _context.SpeakerEvents.Add(speakerEvent);
        }

        public void AddEventSchedule(EventSchedule eventSchedule)
        {
            _context.EventSchedules.Add(eventSchedule);
        }

        public void AddStaffEvent(StaffEvent staffEvent)
        {
            _context.StaffEvents.Add(staffEvent);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public List<EventSchedule> GetAllEventSchedules(int eventId)
        {
            return _context.EventSchedules.Where(es => es.EventId == eventId).ToList();
        }

        public List<SpeakerEvent> GetAllSpeakerEvents(int eventId)
        {
            return _context.SpeakerEvents.Where(se =>  se.EventId == eventId).ToList(); 
        }
        public bool IsSlotOccupied(int venueId, DateOnly eventDate, int slotId)
        {
            // Check if there's already an event at this venue on this date with this slot
            return _context.EventSchedules
                .Include(es => es.Event)
                .Any(es =>
                    es.SlotId == slotId &&
                    es.Event.VenueId == venueId &&
                    es.Event.EventDate == eventDate);
        }
        public bool IsSpeakerOccupiedInSlot(int speakerId, DateOnly eventDate, int slotId)
        {
            // Check if speaker is already assigned to another event on the same date AND same slot
            return _context.SpeakerEvents
                .Include(se => se.Event)
                .Where(se => se.SpeakerId == speakerId && se.Event.EventDate == eventDate)
                .SelectMany(se => _context.EventSchedules.Where(es => es.EventId == se.EventId))
                .Any(es => es.SlotId == slotId);
        }

        public List<Event> GetEventsByOrganizerId(int organizerId)
        {
            return _context.Events
                .Include(e => e.User)
                .Include(e => e.Venue)
                .Where(e => e.UserId == organizerId)
                .OrderByDescending(e => e.EventDate)
                .ToList();
        }
    }


}
