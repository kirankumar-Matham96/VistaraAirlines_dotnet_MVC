using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Helpers
{
    public class SessionHelper
    {
        public static int UserId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Current.Session["userid"]?.ToString() ?? null);
            }

            set
            {
                HttpContext.Current.Session["userid"] = value;
            }
        }

        public static string UserName
        {
            get
            {
                return HttpContext.Current?.Session["username"]?.ToString() ?? null;
            }

            set
            {
                HttpContext.Current.Session["username"] = value;
            }
        }

        public static string UserRole
        {
            get
            {
                return HttpContext.Current.Session["userrole"]?.ToString() ?? null;
            }

            set
            {
                HttpContext.Current.Session["userrole"] = value;
            }
        }
    }
}