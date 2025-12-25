using System.ComponentModel.DataAnnotations;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class UpdateFeedbackRequest
    {
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}