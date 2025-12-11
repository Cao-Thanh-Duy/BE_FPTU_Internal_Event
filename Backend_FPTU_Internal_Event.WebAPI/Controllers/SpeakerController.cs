using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.BLL.Services;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeakerController : ControllerBase
    {
        private readonly ISpeakerService _speakerService;

        public SpeakerController(ISpeakerService speakerService)
        {
            _speakerService = speakerService;
        }




        [HttpPost]
        public IActionResult CreateSpeaker([FromBody] CreateSpeakerRequest request)
        {
            try
            {
                var result = _speakerService.CreateSpeaker(request);
                if (result != null)
                {
                    return Ok(new
                    {

                        success = true,
                        message = "Create Speaker Successfully.",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Internal Server.",

                    });
                }



            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message

                });
            }
        }

        [HttpGet]
        public IActionResult GetAllSpeaker()
        {
            try
            {
                var result = _speakerService.GetAllSpeaker();
                return Ok(new
                {
                    success = true,
                    message = "Speaker retrieved succeesfully",
                    data = result
                });


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetSpeackerById(int id)
        {
            try
            {
                var result = _speakerService.GetSpeakerById(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Speaker with ID {id} not found"
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
        public IActionResult DeleteSpeaker([FromQuery] int speakerId)
        {
            try
            {
                var result = _speakerService.DeleteSpeaker(speakerId);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Speaker deleted successfully"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Delete failed"
                    });
                }
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

        [HttpPut]
        public IActionResult UpdateSpeaker([FromQuery] int speakerId, [FromBody] UpdateSpeakerRequest request)
        {
            try
            {
                var result = _speakerService.UpdateSpeaker(speakerId, request);
                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Update Speaker successfully ",
                        data = result
                    });

                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Internal server"


                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    detail = ex.Message
                });
            }


        }
    }
}
