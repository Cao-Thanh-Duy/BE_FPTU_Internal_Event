using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class CreateEventRequest
    {
        
        public string EventName { get; set; } = string.Empty;

        
        public string EventDescription { get; set; } = string.Empty;

        
        public DateOnly EventDate { get; set; }

        
        public int MaxTicketCount { get; set; }

     
        public int VenueId { get; set; }

       
        public List<int>? SpeakerIds { get; set; }

        
        public List<int>? SlotIds { get; set; }

        
        public List<int>? StaffIds { get; set; }
    }
}
