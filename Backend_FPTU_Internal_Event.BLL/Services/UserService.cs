using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
                    RoleName = user.Role?.RoleName ?? string.Empty,
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
                RoleName = createdUser.Role?.RoleName ?? string.Empty
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
                RoleName = user.Role?.RoleName ?? string.Empty
            };
        }

        
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public UserDTO UpdateUserName(int userId, string userName)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null) throw new KeyNotFoundException($"User id {userId} do not exist");

            user.UserName = userName;
            _userRepository.SaveChanges();

            return new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = user.Role.RoleName

            };
           

        }

        public bool DeleteUser(int userId)
        {
            var user = _userRepository.DeleteUser(userId);

            if (!user)
                throw new KeyNotFoundException($"User with id {userId} does not exist");

            _userRepository.SaveChanges();
            return true;


        }

        public UserDTO? UpdateUserProfileByAdmin(int userId, UpdateUserProfileRequest request)
        {
            // 1. Check if user exists
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            // 2. Validate Role exists
            var role = _userRepository.GetRoleById(request.RoleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {request.RoleId} not found");
            }

            
           

            // 4. Update ONLY UserName and RoleId
            user.UserName = request.UserName;
            user.RoleId = request.RoleId;

            // 5. Save changes
            _userRepository.SaveChanges();

            // 6. Return updated user DTO
            return new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };
        }

        public UserDTO? AddEmailForGoogleLogin(AddEmailRequest request)
        {
            // 1. Check if email already exists
            var existingUser = _userRepository.GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Email '{request.Email}' already exists in the system");
            }

            // 2. Validate Role exists
            var role = _userRepository.GetRoleById(request.RoleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {request.RoleId} not found");
            }

            // 3. Create user with empty password (for Google Login only)
            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                HashPassword = string.Empty, // No password - Google Login only
                RoleId = request.RoleId
            };

            _userRepository.AddUser(newUser);
            _userRepository.SaveChanges();

            // 4. Return created user
            return new UserDTO
            {
                UserId = newUser.UserId,
                UserName = newUser.UserName,
                Email = newUser.Email,
                RoleName = role.RoleName
            };
        }




        //private bool IsValidFptuEmail(string email)
        //{
        //    var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@fptu\.edu\.vn$");
        //    return regex.IsMatch(email);
        //}




    }
}
