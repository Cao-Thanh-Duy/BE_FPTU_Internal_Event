using System.Collections.Generic;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class EventFeedbackSummaryDTO
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
        public List<FeedbackDTO> Feedbacks { get; set; } = new();
    }
}