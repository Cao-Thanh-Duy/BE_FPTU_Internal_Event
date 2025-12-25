using System.ComponentModel.DataAnnotations;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class AddEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "UserName must be between 2 and 100 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "RoleId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0")]
        public int RoleId { get; set; }
    }
}