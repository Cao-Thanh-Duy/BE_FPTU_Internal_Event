using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class VenueDTO
    {
        public int VenueId { get; set; }
        public string VenueName { get;set; }

        public int MaxSeat { get; set; }

        public string LocationDetails { get; set; }

    }
}
