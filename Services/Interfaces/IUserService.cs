using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Services.Interfaces
{
    internal interface IUserService
    {
        void AddUser(UserViewModel user);
        void UserLogin(LoginViewModel user);
    }
}
