USE VISTARA_DB;

-- Users
SELECT * FROM Users;

DELETE FROM Users WHERE UserId IN (2,4,5,6);

-- Flight
SELECT * FROM Flight;
-- FlightInventory
SELECT * FROM FlightInventory;



