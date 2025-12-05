using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public DateOnly EventDate {  get; set; }
        public int MaxTicketCount { get; set; }
        public int CurrentTicketCount { get; set; }
        public string Status { get; set; } = "Pending";

        //
        public int UserId { get; set; }
        public int VenueId { get; set; }

        public User? User { get; set; }
        public Venue? Venue { get; set; }

    }
}
