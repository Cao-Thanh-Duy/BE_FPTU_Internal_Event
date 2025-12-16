using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool UserExistsInEvent(int userId, int eventId)
        {
            return _context.Tickets.Any(t => t.UserId == userId && t.EventId == eventId);
        }

        public bool UserExists(int userId)
        {
            return _context.Users.Any(u => u.UserId == userId);
        }

        public bool EventExists(int eventId)
        {
            return _context.Events.Any(e => e.EventId == eventId);
        }

        public Ticket AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return ticket;
        }

        public List<Ticket> GetTicketByUserId(int userId)
        {
            return _context.Tickets.Where(t => t.UserId == userId).ToList();
        }

        public Ticket GetTicketByTicketId(int tiketId)
        {
            return _context.Tickets.Find(tiketId);
        }

        public Ticket? GetTicketByTicketCode(Guid ticketCode)
        {
            return _context.Tickets.FirstOrDefault(t => t.TicketCode == ticketCode);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public List<Ticket> GetTicketsByEventId(int eventId)
        {
            return _context.Tickets
         .Include(t => t.User)
         .Include(t => t.Event)
         .Where(t => t.EventId == eventId)
         .OrderBy(t => t.SeetNumber)
         .ToList();
        }

        public Ticket? GetActiveTicketByUserAndEvent(int userId, int eventId)
        {
            // Lấy ticket của user tại event này, chỉ lấy ticket chưa bị cancelled
            return _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Event)
                .FirstOrDefault(t => t.UserId == userId &&
                                    t.EventId == eventId &&
                                    t.Status != "Cancelled");
        }

        public int GetNextAvailableSeatNumber(int eventId, int maxTicketCount)
        {
            // Lấy tất cả seat numbers đang được sử dụng (chưa bị cancelled)
            var occupiedSeats = _context.Tickets
                .Where(t => t.EventId == eventId && t.Status != "Cancelled")
                .Select(t => t.SeetNumber)
                .OrderBy(s => s)
                .ToList();

            // Tìm số ghế trống đầu tiên từ 1 đến MaxTicketCount
            for (int seatNumber = 1; seatNumber <= maxTicketCount; seatNumber++)
            {
                if (!occupiedSeats.Contains(seatNumber))
                {
                    return seatNumber;
                }
            }

          
            throw new Exception("No available seat found");
        }
    }
}
