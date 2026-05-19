using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services.Interfaces;

namespace VistaraAirLinesApp.Services
{
    public class UserService : IUserService
    {
        private readonly VISTARA_DBEntities4 _db;
        public UserService()
        {
            _db = new VISTARA_DBEntities4();
        }

        public void AddUser(UserViewModel user)
        {
            try
            {
                var newUser = new User();

                newUser.FullName = $"{user.Firstname} {user.Lastname}";
                newUser.UserName = user.UserName;
                newUser.Email = user.Email;
                newUser.Role = user.Role;
                newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, workFactor: 12);
                newUser.CreatedAt = DateTime.Now;
                newUser.UpdatedAt = DateTime.Now;

                _db.Users.Add(newUser);
                _db.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public User UserLogin(LoginViewModel user)
        {
            try
            {
                var userExists = _db.Users.FirstOrDefault(u => (u.UserName == user.UserId || u.Email == user.UserId) && u.Role == user.Role);

                if (userExists == null)
                {
                    throw new Exception("User not found");
                }

                var passwordMatch = BCrypt.Net.BCrypt.Verify(user.Password, userExists.PasswordHash);

                if (!passwordMatch)
                {
                    throw new Exception("Invalid Password");
                }


                // store user id, user name, and role in the session here...
                return userExists;
            }
            catch
            {
                throw;
            }

        }
    }
}