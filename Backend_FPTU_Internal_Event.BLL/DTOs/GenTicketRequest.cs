using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class GenTicketRequest
    {
        public int UserId { get; set; }
        public int EventId {  get; set; }
    }
}
