using Backend_FPTU_Internal_Event.BLL.DTOs;
using System.Collections.Generic;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface IFeedbackService
    {
        FeedbackDTO CreateFeedback(CreateFeedbackRequest request, int userId);
        FeedbackDTO UpdateFeedback(int feedbackId, UpdateFeedbackRequest request, int userId);
        FeedbackDTO? GetFeedbackByTicketId(int ticketId);
        List<FeedbackDTO> GetMyFeedbacks(int userId);
        List<FeedbackDTO> GetAllFeedbacksByEventId(int eventId);
        EventFeedbackSummaryDTO GetEventFeedbackSummary(int eventId);
    }
}