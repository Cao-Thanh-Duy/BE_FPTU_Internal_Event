using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using SkiaSharp;
using SkiaSharp.QrCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public TicketDTO GenerateTicket(GenTicketRequest request)
        {
            if (!_ticketRepository.UserExists(request.UserId))
            {
                throw new Exception("User do not exist");
            }

            if (!_ticketRepository.EventExists(request.EventId))
            {
                throw new Exception("Event do not exist");
            }

            if (_ticketRepository.UserExistsInEvent(request.UserId, request.EventId))
            {
                throw new Exception("The user has already registered for tickets to this event.");
            }
            var e = _eventRepository.GetEventById(request.EventId);

            if (e.CurrentTicketCount == 0) 
            {
                throw new Exception("Full slot");
            }

            var ticket = new Ticket
            {
                UserId = request.UserId,
                EventId = request.EventId,
                TicketCode = Guid.NewGuid(),
                Status = "Not Used",
                SeetNumber = e.MaxTicketCount - e.CurrentTicketCount + 1
            };
            e.CurrentTicketCount--;
            _eventRepository.SaveChanges();
            var createdTicket = _ticketRepository.AddTicket(ticket);

            return new TicketDTO
            {
                TicketId = createdTicket.TicketId,
                TicketCode = createdTicket.TicketCode,
                Status = createdTicket.Status,
                SeatNumber = createdTicket.SeetNumber
            };
        }

        public string GenerateQrCode(Guid ticketCode)
        {
            try
            {
                // Tạo QR code bằng SkiaSharp.QrCode
                var qrCode = new QRCodeGenerator();
                var qrCodeData = qrCode.CreateQrCode(ticketCode.ToString(), ECCLevel.Q);

                // Tạo image info
                var info = new SKImageInfo(512, 512);
                using var surface = SKSurface.Create(info);
                var canvas = surface.Canvas;

                // Clear background màu trắng
                canvas.Clear(SKColors.White);

                // Render QR code
                canvas.Render(qrCodeData, info.Width, info.Height);

                // Encode thành PNG
                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return Convert.ToBase64String(data.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate QR code: {ex.Message}", ex);
            }
        }

        public List<TicketEventDTO> GetTikets(int userId)
        {
            var user = _userRepository.GetUserById(userId) ?? throw new Exception("User is not exits! ");

            List<Ticket> tickets = _ticketRepository.GetTicketByUserId(user.UserId);
            List<TicketEventDTO> listTickets = new();
            if (tickets != null)
            {

                foreach (var ticket in tickets)
                {
                    var e = _eventRepository.GetEventById(ticket.EventId);
                    var ticketEventDto = new TicketEventDTO
                    {
                        EventName = e.EventName,
                        StartDay = e.EventDate,
                        TicketId = ticket.TicketId,
                        SeatNumber = ticket.SeetNumber,
                        Status = ticket.Status,
                        TicketCode = ticket.TicketCode,
                        UserName = user.UserName
                    };
                    listTickets.Add(ticketEventDto);
                }
                return listTickets;
            }
            else return listTickets;
        }

        public Guid GetTicketCodeByTicketId(int tiketId)
        {
            var ticket = _ticketRepository.GetTicketByTicketId(tiketId) 
                ?? throw new Exception("Ticket is not exits!"); 
            var ticketCode = ticket.TicketCode;
            return ticketCode;
        }

        public TicketEventDTO GetTicketByTicketCode(Guid ticketCode)
        {
            var ticket = _ticketRepository.GetTicketByTicketCode(ticketCode)
                ?? throw new Exception("Ticket is not exist!");
            var user = _userRepository.GetUserById(ticket.UserId) 
                ?? throw new Exception("User is not exits! ");
            var e = _eventRepository.GetEventById(ticket.EventId)
                ?? throw new Exception("Event is not exits! ");
            return new TicketEventDTO
            {
                EventName = e.EventName,
                SeatNumber = ticket.SeetNumber,
                Status = ticket.Status,
                StartDay = e.EventDate,
                TicketCode = ticket.TicketCode,
                TicketId = ticket.TicketId,
                UserName = user.UserName
            };

        }

        public bool CheckIn(int ticketId)
        {
            
            var ticket = _ticketRepository.GetTicketByTicketId(ticketId) ?? throw new Exception("Ticket do not exits");
            if (ticket.Status == "Checked" || ticket.Status == "Cancelled") return false;
        
            var status = "Checked";
            ticket.Status = status;
            _ticketRepository.SaveChanges();
            return true;
            


        }

        public bool Cancelled(int ticketId)
        {
            var ticket = _ticketRepository.GetTicketByTicketId(ticketId) ?? throw new Exception("Ticket do not exits");
            if (ticket.Status == "Checked" || ticket.Status == "Cancelled") return false;

            var status = "Cancelled";
            ticket.Status = status;
            _ticketRepository.SaveChanges();
            return true;
        }
    }
}
