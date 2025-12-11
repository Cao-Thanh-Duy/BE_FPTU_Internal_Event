using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class TicketEventDTO
    {
        public int TicketId { get; set; }
        public Guid TicketCode { get; set; }
        public string Status { get; set; }
        public int SeatNumber { get; set; }
        public string EventName { get; set; }
        public DateOnly StartDay { get; set; }
        public string UserName { get; set; }
    }
}
