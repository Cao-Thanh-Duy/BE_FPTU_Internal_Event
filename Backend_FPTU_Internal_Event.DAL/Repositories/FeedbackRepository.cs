using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Backend_FPTU_Internal_Event.DAL.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Feedback? GetFeedbackByTicketId(int ticketId)
        {
            return _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Event)
                .Include(f => f.Ticket)
                .FirstOrDefault(f => f.TicketId == ticketId);
        }

        public Feedback? GetFeedbackById(int feedbackId)
        {
            return _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Event)
                .Include(f => f.Ticket)
                .FirstOrDefault(f => f.FeedbackId == feedbackId);
        }

        public List<Feedback> GetFeedbacksByEventId(int eventId)
        {
            return _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Event)
                .Include(f => f.Ticket)
                .Where(f => f.EventId == eventId)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();
        }

        public List<Feedback> GetFeedbacksByUserId(int userId)
        {
            return _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Event)
                .Include(f => f.Ticket)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();
        }

        public Feedback AddFeedback(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
            return feedback;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}