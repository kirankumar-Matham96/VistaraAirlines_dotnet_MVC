-- Creating indexes
USE VISTARA_DB;

CREATE INDEX IX_Booking_FlightId ON Booking(FlightId);
CREATE INDEX IX_Passenger_BookingId ON PassengerDetails(BookingId);
CREATE INDEX IX_Passenger_FlightDate ON PassengerDetails(FlightId, TravelDate);

