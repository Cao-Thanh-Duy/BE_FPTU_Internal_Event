using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        public VenueService(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public VenueDTO? CreateVenue(CreateVenueRequest request)
        {
            var newVenue = new Venue
            {
                VenueName = request.VenueName,
                MaxSeat = request.MaxSeat,
                LocationDetails = request.LocationDetails

            };

            _venueRepository.AddVenue(newVenue);
            _venueRepository.SaveChanges();

            return new VenueDTO
            {
                VenueName = newVenue.VenueName,
                MaxSeat = newVenue.MaxSeat,
                LocationDetails = newVenue.LocationDetails
            };
        }

        public List<VenueDTO> GetAllVenue()
        {
            List<VenueDTO> listVenueDTO = new();
            var listVenue = _venueRepository.GetAllVenue();
            foreach (var venue in listVenue)
            {
                var venueDTO = new VenueDTO()
                {
                    VenueId = venue.VenueId,
                    VenueName = venue.VenueName,
                    MaxSeat = venue.MaxSeat,
                    LocationDetails = venue.LocationDetails
                };
                listVenueDTO.Add(venueDTO);
                
            }
            return listVenueDTO;
        }


        public VenueDTO? GetVenueById(int venueId)
        {
            var venue = _venueRepository.GetVenueById(venueId);
            if (venue == null)
                return null;

            return new VenueDTO
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                MaxSeat = venue.MaxSeat,
                LocationDetails = venue.LocationDetails
            };
        }
    }
}
