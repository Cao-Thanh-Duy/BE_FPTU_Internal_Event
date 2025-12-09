using Backend_FPTU_Internal_Event.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface IVenueRepository
    {
        List<Venue> GetAllVenue();
        Venue? AddVenue(Venue venue);
        Venue? GetVenueById(int venueId);
        void SaveChanges();
    }
}
