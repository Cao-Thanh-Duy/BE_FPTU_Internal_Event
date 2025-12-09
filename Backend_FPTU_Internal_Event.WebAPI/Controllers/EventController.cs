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
            Summary = "Get Event by ID",
            Description = "Retrieve a specific event by its ID"
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
                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid input",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

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