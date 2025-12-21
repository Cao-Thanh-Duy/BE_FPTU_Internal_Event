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
        void RemoveSpeakerEvent(SpeakerEvent speakerEvent);
        void RemoveEventSchedule(EventSchedule eventSchedule);
        void RemoveStaffEvent(StaffEvent staffEvent);
        List<EventSchedule> GetAllEventSchedules(int eventId);
        List<SpeakerEvent> GetAllSpeakerEvents(int eventId);


        List<StaffEvent> GetAllStaffEvents(int eventId);
        void SaveChanges();
        bool IsSlotOccupied(int venueId, DateOnly eventDate, int slotId);
        bool IsSlotOccupiedExcludeEvent(int venueId, DateOnly eventDate, int slotId, int excludeEventId);
        bool IsSpeakerOccupiedInSlot(int speakerId, DateOnly eventDate, int slotId);
        bool IsSpeakerOccupiedInSlotExcludeEvent(int speakerId, DateOnly eventDate, int slotId, int excludeEventId);
        bool IsStaffOccupiedInSlot(int staffId, DateOnly eventDate, int slotId);
        bool IsStaffOccupiedInSlotExcludeEvent(int staffId, DateOnly eventDate, int slotId, int excludeEventId);
        List<Event> GetEventsByOrganizerId(int organizerId);
        List<Event> GetEventsByStaffId(int staffId);

        bool IsSlotOccupiedExcludeRejected(int venueId, DateOnly eventDate, int slotId);
    }
}
