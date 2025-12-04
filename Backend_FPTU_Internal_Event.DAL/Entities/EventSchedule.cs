using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class EventSchedule
    {
        public int SlotId { get; set; }
        public int EventId { get; set; }

        public required Slot Slot { get; set; } 
        public required Event Event { get; set; }

    }
}
