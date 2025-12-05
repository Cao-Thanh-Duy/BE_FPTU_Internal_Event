using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserDTO> GetAllUser()
        {
            List<UserDTO> listUserDTO = new();
            var listUser = _userRepository.GetAllUser();
            foreach(var user in listUser)
            {
                var userDTO = new UserDTO()
                {
                    Email = user.Email,
                    RoleName = user.Role.RoleName,
                    UserId = user.UserId,
                    UserName = user.UserName
                };
                listUserDTO.Add(userDTO);
            }
            return listUserDTO;
        }

        public UserDTO? CreateUser(CreateUserRequest request)
        {
            // Check if email already exists
            var existingUser = _userRepository.GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                return null; // Email already exists
            }

            // Hash password
            string hashedPassword = HashPassword(request.Password);

            // Create new user
            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                HashPassword = hashedPassword,
                RoleId = request.RoleId
            };

            _userRepository.AddUser(newUser);
            _userRepository.SaveChanges();

            // Reload user with Role information
            var createdUser = _userRepository.GetUserById(newUser.UserId);
            if (createdUser == null)
                return null;

            return new UserDTO
            {
                UserId = createdUser.UserId,
                UserName = createdUser.UserName,
                Email = createdUser.Email,
                RoleName = createdUser.Role.RoleName
            };
        }

        public UserDTO? GetUserById(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = user.Role.RoleName
            };
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
