using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class EventAttendeeDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public Guid TicketCode { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class EventAttendeesResponse
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int TotalAttendees { get; set; }
        public List<EventAttendeeDTO> Attendees { get; set; } = new();
    }
}