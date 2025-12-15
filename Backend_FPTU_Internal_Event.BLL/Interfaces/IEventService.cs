using Backend_FPTU_Internal_Event.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface IEventService
    {
        List<EventDTO> GetAllEvents();
        EventDTO? GetEventById(int eventId);
        EventDTO? CreateEvent(CreateEventRequest request, int organizerId);

        EventDTO? UpdateEvent(int eventId, CreateUpdateEventRequest request);
        List<EventDTO> GetEventsByOrganizerId(int organizerId);
        bool ApproveEvent(int eventId);

        bool RejectEvent(int enentId);

        List<EventDTO> GetEventsByStaffId(int staffId);
    }
}
