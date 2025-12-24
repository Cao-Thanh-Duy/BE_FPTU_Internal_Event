using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface IUserService
    {
        List<UserDTO> GetAllUser();
        UserDTO? CreateUser(CreateUserRequest request);
        UserDTO? GetUserById(int userId);

        UserDTO? UpdateUserName (int userId ,string userName);

        bool DeleteUser(int userId);

        
        UserDTO? UpdateUserProfileByAdmin(int userId, UpdateUserProfileRequest request);
    
}
}
