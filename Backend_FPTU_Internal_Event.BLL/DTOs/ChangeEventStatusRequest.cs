using System.ComponentModel.DataAnnotations;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class ChangeEventStatusRequest
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Approve|Reject)$", ErrorMessage = "Status must be either 'Approve' or 'Reject'")]
        public string Status { get; set; } = string.Empty;
    }
}