using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public Guid TicketCode { get; set; }
        public int SeetNumber { get; set; }
        public string Status { get; set; } = string.Empty;

        //
        public int UserId { get; set; }
        public int EventId { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }
    }
}
