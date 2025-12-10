using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.BLL.Services;
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


        [HttpGet("{id}")]
        public IActionResult GetVenueById(int id)
        {
            try
            {
                var result = _venueService.GetVenueById(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Venue with ID {id} not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User retrieved successfully",
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

        [HttpDelete]
        public IActionResult DeleteVenue(int venueId)
        {
            try
            {
                var result = _venueService.DeleteVenue(venueId);
                if(result == true)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Delete Venue Successfully",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Venue being used in the Event",
                      
                    });
                }
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

    }
}
