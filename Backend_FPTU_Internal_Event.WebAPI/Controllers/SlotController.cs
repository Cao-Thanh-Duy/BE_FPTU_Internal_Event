using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.BLL.Services;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotController(ISlotService slotService)
        {
            _slotService = slotService;
        }
        [HttpPost]
        public IActionResult CreateSlot([FromBody] CreateSlotRequest request)
        {
            try
            {
                var result = _slotService.CreateSlot(request);

                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Create Slot succesfully",
                        data = result

                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to Create Slot",

                    });
                }
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
        }


        [HttpGet]
        public IActionResult GetAllSlot()
        {
            try
            {
                var result = _slotService.GetAllSlot();
                return Ok(new
                {
                    success = true,
                    message = "Slot retrieved successfully",
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
        public IActionResult GetSlotById(int id)
        {
            try
            {
                var result = _slotService.GetSlotById(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Slot with ID {id} not found"
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

        [HttpPut]
        public IActionResult UpdateSlot([FromQuery] int slotId, [FromBody] CreateUpdateSlotRequest request)
        {
            try
            {
                var result = _slotService.UpdateSlot(slotId, request);
                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Slot updated successfully",
                        data = result
                    });

                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to update Slot"
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


        [HttpDelete]
        public IActionResult DeleteSlot([FromQuery] int slotId)
        {
            try
            {
                var result = _slotService.DeleteSlot(slotId);
                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Delete Slot successfully ",
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
                    message = ex.Message

                });
            }
        }
    }
}
