using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public int Rating { get; set; } // 1-5 stars

        public string? Comment { get; set; } // Optional comment

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }

        // Navigation Properties
        public Ticket? Ticket { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }
    }
}