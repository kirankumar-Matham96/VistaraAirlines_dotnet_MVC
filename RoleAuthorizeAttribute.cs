using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VistaraAirLinesApp.Helpers;

namespace VistaraAirLinesApp.CustomFilters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _role;

        public RoleAuthorizeAttribute(string role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //var role = httpContext.Session[_role];
            var role = SessionHelper.UserRole;

            if (role == null)
            {
                return false;
            }

            return role.ToString() == _role;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "User" }, { "action", "LoginUser" } });
        }

    }
}