using System;

namespace VistaraAirLinesApp.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetExceptionMessage(Exception ex)
        {
            return ex.GetBaseException().Message;
        }
    }
}