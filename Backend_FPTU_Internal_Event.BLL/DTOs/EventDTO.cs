using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class EventDTO
    {

        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public DateOnly EventDay { get; set; }
        public int MaxTickerCount { get; set; }
        public int CurrentTickerCount { get; set; }
        public string Status { get; set; } = string.Empty;

        //Venue
        public string VenueName { get; set; } = string.Empty;
        public string LocationDetails { get; set; } = string.Empty;

        //Slot
        public string SlotName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        //Speaker
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerDescription { get; set; } = string.Empty;
    }
}
