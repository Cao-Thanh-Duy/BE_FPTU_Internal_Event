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
    }


}
