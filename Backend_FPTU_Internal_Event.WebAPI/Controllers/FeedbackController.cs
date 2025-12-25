using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Submit Feedback for Checked Ticket",
            Description = "Student can submit feedback after their ticket has been checked. One feedback per ticket. Rating: 1-5 stars."
        )]
        public IActionResult CreateFeedback([FromBody] CreateFeedbackRequest request)
        {
            try
            {
                // Get userId from JWT token
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid user token"
                    });
                }

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

                var result = _feedbackService.CreateFeedback(request, userId);

                return Ok(new
                {
                    success = true,
                    message = "Feedback submitted successfully. Thank you for your feedback!",
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

        [Authorize]
        [HttpPut("{feedbackId}")]
        [SwaggerOperation(
            Summary = "Update Feedback",
            Description = "User can update their own feedback. Can only update Rating and Comment."
        )]
        public IActionResult UpdateFeedback(int feedbackId, [FromBody] UpdateFeedbackRequest request)
        {
            try
            {
                // Get userId from JWT token
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid user token"
                    });
                }

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

                var result = _feedbackService.UpdateFeedback(feedbackId, request, userId);

                return Ok(new
                {
                    success = true,
                    message = "Feedback updated successfully",
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

        [Authorize]
        [HttpGet("my-feedbacks")]
        [SwaggerOperation(
            Summary = "Get My Feedbacks",
            Description = "Retrieve all feedbacks submitted by the current user"
        )]
        public IActionResult GetMyFeedbacks()
        {
            try
            {
                // Get userId from JWT token
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid user token"
                    });
                }

                var result = _feedbackService.GetMyFeedbacks(userId);

                return Ok(new
                {
                    success = true,
                    message = $"Retrieved {result.Count} feedbacks",
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

        [HttpGet("ticket/{ticketId}")]
        [SwaggerOperation(
            Summary = "Get Feedback by Ticket ID",
            Description = "Check if a ticket has feedback"
        )]
        public IActionResult GetFeedbackByTicketId(int ticketId)
        {
            try
            {
                var result = _feedbackService.GetFeedbackByTicketId(ticketId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No feedback found for this ticket"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Feedback retrieved successfully",
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

        [HttpGet("event/{eventId}")]
        [SwaggerOperation(
            Summary = "Get All Feedbacks for Event",
            Description = "Retrieve all feedbacks for a specific event (list view)"
        )]
        public IActionResult GetAllFeedbacksByEvent(int eventId)
        {
            try
            {
                var result = _feedbackService.GetAllFeedbacksByEventId(eventId);

                return Ok(new
                {
                    success = true,
                    message = $"Retrieved {result.Count} feedbacks for event",
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

        [Authorize(Roles = "Admin,Organizer")]
        [HttpGet("event/{eventId}/summary")]
        [SwaggerOperation(
            Summary = "Get Event Feedback Summary",
            Description = "Get statistical summary of all feedbacks for an event (Admin/Organizer only)"
        )]
        public IActionResult GetEventFeedbackSummary(int eventId)
        {
            try
            {
                var result = _feedbackService.GetEventFeedbackSummary(eventId);

                return Ok(new
                {
                    success = true,
                    message = "Feedback summary retrieved successfully",
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