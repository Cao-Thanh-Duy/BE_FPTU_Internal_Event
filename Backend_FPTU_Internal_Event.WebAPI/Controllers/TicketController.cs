using Azure.Core;
using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Buffers.Text;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {

        private readonly ITicketService _ticketService;
        private readonly IEventService _eventService;
        public TicketController(ITicketService ticketService, IEventService eventService)
        {
            _ticketService = ticketService;
            _eventService = eventService;
        }

        [HttpPost]
        public IActionResult GenerationTicket(GenTicketRequest request)
        {
            try
            {
                var result = _ticketService.GenerateTicket(request);
                var qrBase64 = _ticketService.GenerateQrCode(result.TicketCode);
                return Ok(new
                {
                    success = true,
                    message = "Ticket registration successful.",
                    data = result,
                    qr = $"data:image/png;base64,{qrBase64}"
                });
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
        [SwaggerOperation(
            Summary = "Get ticket by UserId",
            Description = ""
        )]
        public IActionResult GetTickets([FromQuery]int userId)
        {
            try
            {
                var result = _ticketService.GetTikets(userId);
                return Ok(new
                {
                    success = true,
                    message = "Ticket retrieved successful.",
                    data = result,
                });
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

        [HttpGet("/getQR")]
        public IActionResult GenarateQRCodeByTicket([FromQuery]int ticketId)
        {
            try
            {
                var result = _ticketService.GetTicketCodeByTicketId(ticketId);
                var qrBase64 = _ticketService.GenerateQrCode(result);
                return Ok(new
                {
                    success = true,
                    message = "Ticket retrieved successful.",
                    qrcode = $"data:image/png;base64,{qrBase64}"
                });
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

        [HttpGet("/infoticket")]
        public IActionResult GetInfoByQRCode([FromQuery] Guid ticketCode)
        {
            try
            {
                var result = _ticketService.GetTicketByTicketCode(ticketCode);
                return Ok(new
                {
                    success = true,
                    message = "Ticket retrieved successful.",
                    data = result,  
                });
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


        [HttpPut("/CheckIn")]
        public IActionResult CheckIn([FromQuery] int ticketId)
        {
            try
            {
               
                var result = _ticketService.CheckIn(ticketId);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Ticket Checked successful.",
                        data = result,
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"This ticket already Checked.",
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

        [HttpPut("/Cancelled")]
        public IActionResult CancelledTicket(int ticketId)
        {
            try
            {

                var result = _ticketService.Cancelled(ticketId);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Ticket Cancelled successful.",
                        data = result,
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"This ticket can not Cancelled.",
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
