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

        public bool CheckExitVenueInEvent(int venueId)
        {
            return _context.Events.Any(e => e.VenueId == venueId);
        }

        public bool DeleteVenue(int venueId)
        {
            var loadVenue = _context.Venues.Find(venueId);
            if (loadVenue != null)
            {
                _context.Venues.Remove(loadVenue);
                _context.SaveChanges();
                return true;
            }
            else return false;
        }

        public bool VenueNameExists(string venueName)
        {
            // Remove all spaces and convert to lowercase for comparison
            var normalizedName = venueName.Replace(" ", "").ToLower();

            return _context.Venues.Any(v =>
                v.VenueName.Replace(" ", "").ToLower() == normalizedName);
        }

        public bool VenueNameExistsExcludeVenue(string venueName, int excludeVenueId)
        {
            // Remove all spaces and convert to lowercase for comparison
            var normalizedName = venueName.Replace(" ", "").ToLower();

            return _context.Venues.Any(v =>
                v.VenueId != excludeVenueId &&
                v.VenueName.Replace(" ", "").ToLower() == normalizedName);
        }
    }
}
