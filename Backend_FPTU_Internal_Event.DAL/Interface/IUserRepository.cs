using Backend_FPTU_Internal_Event.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface IUserRepository
    {
        List<User> GetAllUser();
        User? Login(string username, string password);
        User? GetUserByEmail(string email);

        User? GetUserById(int userId);
        void AddUser(User user);
        void SaveChanges();

        bool DeleteUser(int userId);

       
        bool EmailExists(string email);
        bool EmailExistsExcludeUser(string email, int excludeUserId);
        Role? GetRoleById(int roleId);
       
    }
}
