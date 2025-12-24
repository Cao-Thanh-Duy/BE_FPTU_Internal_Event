using Backend_FPTU_Internal_Event.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface IAuthService
    {
        LoginResponse? Login(LoginRequest request);
        Task<LoginResponse?> GoogleLogin(GoogleLoginRequest request);
        string GenerateJwtToken(int userId, string email, string roleName);
    }
}
