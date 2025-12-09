using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly ApplicationDbContext _context;
        public VenueRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Venue? AddVenue(Venue venue)
        {
           _context.Venues.Add(venue);
            return venue;

        }

        public List<Venue> GetAllVenue()
        {
            return _context.Venues.ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Venue? GetVenueById(int venueId)
        {
            return _context.Venues.FirstOrDefault(v => v.VenueId == venueId);
        }
    }
}
