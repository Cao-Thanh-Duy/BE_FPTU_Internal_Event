using Backend_FPTU_Internal_Event.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface IEventRepository
    {
        List<Event> GetAllEvents();
        Event? GetEventById(int eventId);
        void AddEvent(Event eventEntity);
        void AddSpeakerEvent(SpeakerEvent speakerEvent);
        void AddEventSchedule(EventSchedule eventSchedule);
        void AddStaffEvent(StaffEvent staffEvent);
        void SaveChanges();
    }
}
