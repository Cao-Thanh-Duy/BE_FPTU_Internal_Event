using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.BLL.Services;
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
    }
}
