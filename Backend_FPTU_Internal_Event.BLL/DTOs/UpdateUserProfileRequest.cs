using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class UpdateUserProfileRequest
    {
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(100, ErrorMessage = "UserName cannot exceed 100 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "RoleId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be a positive number")]
        public int RoleId { get; set; }
    }
}