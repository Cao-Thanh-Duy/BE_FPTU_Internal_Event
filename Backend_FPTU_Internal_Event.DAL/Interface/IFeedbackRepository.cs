using Backend_FPTU_Internal_Event.DAL.Entities;
using System.Collections.Generic;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface IFeedbackRepository
    {
        Feedback? GetFeedbackByTicketId(int ticketId);
        Feedback? GetFeedbackById(int feedbackId);
        List<Feedback> GetFeedbacksByEventId(int eventId);
        List<Feedback> GetFeedbacksByUserId(int userId);
        Feedback AddFeedback(Feedback feedback);
        void SaveChanges();
    }
}