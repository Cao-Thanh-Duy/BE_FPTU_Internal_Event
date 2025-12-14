using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        
        /// Get all events
      
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get All Events",
            Description = "Retrieve all events in the system"
        )]
        public IActionResult GetAllEvents()
        {
            try
            {
                var result = _eventService.GetAllEvents();
                return Ok(new
                {
                    success = true,
                    message = "Events retrieved successfully",
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

       
        /// Get event by ID
      
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get Event by EventID",
            Description = "Retrieve a specific event by eventID"
        )]
        public IActionResult GetEventById(int id)
        {
            try
            {
                var result = _eventService.GetEventById(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Event with ID {id} not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Event retrieved successfully",
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

        
        /// Create new event - Admin/Organizer only
     
        //[Authorize(Roles = "Admin,Organizer")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create New Event",
            Description = "Create a new event with venue, speakers, slots, and staff assignments - Admin/Organizer only"
        )]
        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
        {
            try
            {

                // Get current user ID from JWT token
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int organizerId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid user token"
                    });
                }

                var result = _eventService.CreateEvent(request, organizerId);

                if (result == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to create event"
                    });
                }

                return CreatedAtAction(
                    nameof(GetEventById),
                    new { id = result.EventId },
                    new
                    {
                        success = true,
                        message = "Event created successfully",
                        data = result
                    });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new
                {
                    success = false,
                    message = knfEx.Message
                });
            }
            catch (InvalidOperationException ioEx)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ioEx.Message
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


        //[HttpPut]
        //public IActionResult UpdateEvent([FromQuery] int eventId, CreateUpdateEventRequest request)
        //{

        //}

        [HttpGet("my-events")]
        [SwaggerOperation(
            Summary = "Get My Created Events",
         Description = "Retrieve all events created by  (EventOrg/Admin)"
         )]
        public IActionResult GetEventsByOrganizerId(int organizerId)
        {
            try
            {
               

                var result = _eventService.GetEventsByOrganizerId(organizerId);

                return Ok(new
                {
                    success = true,
                    message = $"Retrieved {result.Count} events created by you",
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




        [HttpPut("/Approve")]
        public IActionResult ApproveEvent(int eventId)
        {
            try
            {
                var result = _eventService.ApproveEvent(eventId);
                if (result == true)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Approve Event Successfully ",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to Approve Event"


                    });

                }
            }
            catch (KeyNotFoundException knfEx)
            {
                return BadRequest(new
                {
                    success = false,
                    message = knfEx.Message
                });
            }
        }

        [HttpPut("/Reject")]
        public IActionResult RejectEvent(int eventId)
        {
            try
            {
                var result = _eventService.RejectEvent(eventId);
                if(result == true)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Reject Event Successfully ",
                        data = result
                    });

                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to Reject Event"
                    });
                }
            }catch(KeyNotFoundException knfEx)
            {
                return BadRequest(new
                {
                    success = false,
                    message = knfEx.Message
                });
            }
        }
    }
}