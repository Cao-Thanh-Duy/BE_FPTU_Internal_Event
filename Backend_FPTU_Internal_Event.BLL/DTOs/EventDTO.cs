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
       public string EventName { get; set; }= string.Empty;
       public string EventDescription { get; set; }
       

    }
}
