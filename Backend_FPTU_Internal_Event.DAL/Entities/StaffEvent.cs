using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class StaffEvent
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public required Event Event { get; set; } 
        public required User User { get; set; }
    }
}
