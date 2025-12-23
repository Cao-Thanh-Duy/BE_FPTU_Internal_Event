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
            // Validate VenueName is not empty
            if (string.IsNullOrWhiteSpace(request.VenueName))
            {
                throw new ArgumentException("Venue name cannot be empty");
            }

            // Check if venue name already exists (case-insensitive, ignore spaces)
            if (_venueRepository.VenueNameExists(request.VenueName))
            {
                throw new InvalidOperationException($"Venue with name '{request.VenueName}' already exists. Venue names must be unique (case-insensitive, spaces ignored).");
            }

            var newVenue = new Venue
            {
                VenueName = request.VenueName.Trim(), // Trim leading/trailing spaces
                MaxSeat = request.MaxSeat,
                LocationDetails = request.LocationDetails
            };

            _venueRepository.AddVenue(newVenue);
            _venueRepository.SaveChanges();

            return new VenueDTO
            {
                VenueId = newVenue.VenueId,
                VenueName = newVenue.VenueName,
                MaxSeat = newVenue.MaxSeat,
                LocationDetails = newVenue.LocationDetails
            };
        }

        public bool DeleteVenue(int VenueId)
        {
            if(_venueRepository.CheckExitVenueInEvent(VenueId) == true)
            {
                return false;
            }
            else
            {
                var venue = _venueRepository.DeleteVenue(VenueId);
                return true;
            }
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

        public VenueDTO? UpdateVenue(int venueId, CreateUpdateVenueRequest request)
        {
            var venue = _venueRepository.GetVenueById(venueId)
                ?? throw new KeyNotFoundException($"Venue with ID {venueId} does not exist");

            // Validate VenueName is not empty
            if (string.IsNullOrWhiteSpace(request.VenueName))
            {
                throw new ArgumentException("Venue name cannot be empty");
            }

            // Check if new venue name conflicts with existing venues (excluding current venue)
            if (_venueRepository.VenueNameExistsExcludeVenue(request.VenueName, venueId))
            {
                throw new InvalidOperationException($"Venue with name '{request.VenueName}' already exists. Venue names must be unique (case-insensitive, spaces ignored).");
            }

            venue.VenueName = request.VenueName.Trim();
            venue.MaxSeat = request.MaxSeat;
            venue.LocationDetails = request.LocationDetails;

            _venueRepository.SaveChanges();

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
