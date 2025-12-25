using Backend_FPTU_Internal_Event.BLL.DTOs;
using System.Collections.Generic;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface IFeedbackService
    {
        FeedbackDTO CreateFeedback(CreateFeedbackRequest request, int userId);
        FeedbackDTO? GetFeedbackByTicketId(int ticketId);
        List<FeedbackDTO> GetMyFeedbacks(int userId);
        EventFeedbackSummaryDTO GetEventFeedbackSummary(int eventId);
    }
}