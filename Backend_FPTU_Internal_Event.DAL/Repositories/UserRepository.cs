using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Repositories
{

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUser()
        {
            return _context.Users.Include(u => u.Role).ToList();
        }

        public User? Login(string username, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u =>
                    u.Email == username &&
                    u.HashPassword == password);

            return user;
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);
        }

        public User? GetUserById(int userId)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == userId);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool DeleteUser(int userId)
        {
            var loadUser = _context.Users.Find(userId);
            if (loadUser != null)
            {
                _context.Users.Remove(loadUser);
                return true;
            }
            else return false;
        }

        public User? GetUserByUserName(string userName)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserName == userName);
        }
    }
}