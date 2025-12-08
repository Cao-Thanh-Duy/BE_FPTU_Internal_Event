using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }


        [HttpPost]
        public IActionResult CreateVenue([FromBody] CreateVenueRequest request)
        {
            try
            {
                var result = _venueService.CreateVenue(request);
                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Create Venue succesfully",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Create Venue succesfully",
                        
                    });
                }
            }
            catch (Exception ex)
            {
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = ex.Message,

                    });
                }
            }

        }

        [HttpGet]
        public IActionResult GetAllVenue()
        {
            try
            {
                var result = _venueService.GetAllVenue();
                return Ok(new
                {
                    success = true,
                    message = "Venue retrieved successfully",
                    data = result
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

    }
}
