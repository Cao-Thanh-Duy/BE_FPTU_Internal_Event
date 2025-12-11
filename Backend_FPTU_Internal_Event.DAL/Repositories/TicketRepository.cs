using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
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
    }
}
