using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class SpeakerEvent
    {
        public int SpeakerId { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}
