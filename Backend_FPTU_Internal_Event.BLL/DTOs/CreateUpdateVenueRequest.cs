using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class CreateUpdateVenueRequest
    {
        public string VenueName { get; set; } = string.Empty;
        public int MaxSeat { get; set; }
        public string LocationDetails { get; set; } = string.Empty;
    }
}
