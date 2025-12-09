using Backend_FPTU_Internal_Event.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface IVenueService
    {
        VenueDTO? CreateVenue(CreateVenueRequest request);
        List<VenueDTO> GetAllVenue();

        VenueDTO? GetVenueById(int VenueId);
    }
}
