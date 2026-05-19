using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Services.Interfaces
{
    internal interface IFlightService
    {
        (List<int> HHRange, List<int> MMRange, List<int> SSRange) GetTimeRanges();
        void AddFlight(FlightViewModel flight);
        Flight GetFlightById(int id);
        FlightInventory GetFlightInventoryById(int id);
        List<FlightViewModel> GetAllFlights();
        FlightViewModel GetFullFlightDetails(int id);
        void UpdateFlightDetails(FlightViewModel flight);
        void DeleteFlight(int id);
    }
}
