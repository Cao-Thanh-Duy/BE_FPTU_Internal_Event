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
        private readonly ISlotRepository _slotRepository;
        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, IUserRepository userRepository, ISlotRepository slotRepository)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _slotRepository = slotRepository;
        }

        public TicketDTO GenerateTicket(GenTicketRequest request)
        {
            // Validate user exists
            if (!_ticketRepository.UserExists(request.UserId))
            {
                throw new Exception("User does not exist");
            }

            // Validate event exists
            if (!_ticketRepository.EventExists(request.EventId))
            {
                throw new Exception("Event does not exist");
            }

            // Check if user already has an ACTIVE ticket (Not Used or Checked) for this event
            var existingActiveTicket = _ticketRepository.GetActiveTicketByUserAndEvent(request.UserId, request.EventId);
            if (existingActiveTicket != null)
            {
                throw new Exception($"You already have a ticket for this event with status '{existingActiveTicket.Status}'. Cannot purchase another ticket.");
            }

            // Get event
            var e = _eventRepository.GetEventById(request.EventId);

            // Check if event has available tickets
            if (e.CurrentTicketCount == 0)
            {
                throw new Exception("Full slot - No tickets available");
            }

            // Check if event status is Approved
            if (e.Status != "Approve")
            {
                throw new Exception($"Cannot purchase ticket. Event status is '{e.Status}'. Only approved events allow ticket purchases.");
            }

            // Get next available seat number (tìm ghế trống đầu tiên)
            int seatNumber = _ticketRepository.GetNextAvailableSeatNumber(request.EventId, e.MaxTicketCount);

            // Create new ticket
            var ticket = new Ticket
            {
                UserId = request.UserId,
                EventId = request.EventId,
                TicketCode = Guid.NewGuid(),
                Status = "Not Used",
                SeetNumber = seatNumber  // Sử dụng seat number từ logic tìm ghế trống
            };

            // Decrease available ticket count
            e.CurrentTicketCount--;
            _eventRepository.SaveChanges();

            // Add ticket to database
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

                    // Get all slots for this event
                    List<EventSlotDTO> slotEventDto = new();
                    var slotEvent = _eventRepository.GetAllEventSchedules(ticket.EventId);
                    foreach (var slot in slotEvent)
                    {
                        var slotdto = _slotRepository.GetSlotById(slot.SlotId);
                        if (slotdto != null)
                        {
                            EventSlotDTO evslot = new()
                            {
                                StartTime = slotdto.StartTime,
                                EndTime = slotdto.EndtTime,
                                SlotName = slotdto.SlotName
                            };
                            slotEventDto.Add(evslot);
                        }
                    }

                    var ticketEventDto = new TicketEventDTO
                    {
                        EventName = e.EventName,
                        StartDay = e.EventDate,
                        TicketId = ticket.TicketId,
                        SeatNumber = ticket.SeetNumber,
                        Status = ticket.Status,
                        TicketCode = ticket.TicketCode,
                        UserName = user.UserName,
                        Slots = slotEventDto  // Add slots to response
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

            // Get all slots for this event
            List<EventSlotDTO> slotEventDto = new();
            var slotEvent = _eventRepository.GetAllEventSchedules(ticket.EventId);
            foreach (var slot in slotEvent)
            {
                var slotdto = _slotRepository.GetSlotById(slot.SlotId);
                if (slotdto != null)
                {
                    EventSlotDTO evslot = new()
                    {
                        StartTime = slotdto.StartTime,
                        EndTime = slotdto.EndtTime,
                        SlotName = slotdto.SlotName
                    };
                    slotEventDto.Add(evslot);
                }
            }

            return new TicketEventDTO
            {
                EventName = e.EventName,
                SeatNumber = ticket.SeetNumber,
                Status = ticket.Status,
                StartDay = e.EventDate,
                TicketCode = ticket.TicketCode,
                TicketId = ticket.TicketId,
                UserName = user.UserName,
                Slots = slotEventDto  // Add slots to response
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
            // Get ticket
            var ticket = _ticketRepository.GetTicketByTicketId(ticketId)
                ?? throw new Exception("Ticket does not exist");

            // Check if ticket can be cancelled
            if (ticket.Status == "Checked")
            {
                throw new Exception("Cannot cancel ticket. Ticket has already been checked in.");
            }

            if (ticket.Status == "Cancelled")
            {
                throw new Exception("Ticket has already been cancelled.");
            }

            // Get event to increase available ticket count
            var eventEntity = _eventRepository.GetEventById(ticket.EventId);
            if (eventEntity == null)
            {
                throw new Exception("Event not found");
            }

            // Cancel ticket
            ticket.Status = "Cancelled";

            // Increase available ticket count back
            eventEntity.CurrentTicketCount++;

            // Save changes
            _eventRepository.SaveChanges();
            _ticketRepository.SaveChanges();

            return true;
        }

        public EventAttendeesResponse GetEventAttendees(int eventId)
        {
            // Check if event exists
            var eventEntity = _eventRepository.GetEventById(eventId);
            if (eventEntity == null)
            {
                throw new Exception($"Event with ID {eventId} not found");
            }

            // Get all tickets for this event
            var tickets = _ticketRepository.GetTicketsByEventId(eventId);

            // Map to DTOs
            var attendees = tickets.Select(t => new EventAttendeeDTO
            {
                UserId = t.UserId,
                UserName = t.User?.UserName ?? string.Empty,
                Email = t.User?.Email ?? string.Empty,
                TicketId = t.TicketId,
                TicketCode = t.TicketCode,
                SeatNumber = t.SeetNumber,
                Status = t.Status
            }).ToList();

            return new EventAttendeesResponse
            {
                EventId = eventId,
                EventName = eventEntity.EventName,
                TotalAttendees = attendees.Count,
                Attendees = attendees
            };
        }
    }
}
