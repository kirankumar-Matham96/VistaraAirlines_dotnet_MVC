# ✈️ Vistara Airlines Management System

A full-stack **ASP.NET MVC web application** that simulates an airline booking system.
This project demonstrates real-world features like flight search, booking, passenger management, and cancellation with a structured and scalable database design.

---

## 🚀 Features

* 🔍 Search available flights
* 📅 View flight schedules and availability
* 🎫 Book tickets for multiple passengers
* 💺 Seat selection with validation
* ❌ Cancel bookings with refund calculation
* 👤 Role-based login (Manager / Employee)
* 📊 Flight inventory management (seats & fares)

---

## 🛠️ Tech Stack

* **Frontend:** HTML, CSS, Bootstrap, JavaScript
* **Backend:** ASP.NET MVC (C#)
* **Database:** SQL Server
* **ORM:** Entity Framework (DB First / Code First)
* **Version Control:** Git & GitHub

---

## 🧱 Database Design

The project follows **normalized and production-ready database design**:

* `Flight` → Master flight data
* `FlightInventory` → Seat availability per date
* `Booking` → Booking transactions
* `PassengerDetails` → Passenger-level data
* `Cancellation` → Refund & cancellation tracking
* `User` → Authentication & role management

---

## 📸 Screenshots

> *(Add your screenshots after completing UI — examples below)*

### 🔹 Home Page

![Home Page](screenshots/home.png)

### 🔹 Flight Search

![Flight Search](screenshots/search.png)

### 🔹 Booking Page

![Booking](screenshots/booking.png)

### 🔹 Passenger Details

![Passenger](screenshots/passenger.png)

### 🔹 Booking Confirmation

![Confirmation](screenshots/confirmation.png)

### 🔹 Cancellation Page

![Cancellation](screenshots/cancellation.png)

---

## ⚙️ Setup Instructions

### 1️⃣ Clone the repository

```bash
git clone https://github.com/yourusername/vistara-airlines.git
cd vistara-airlines
```

### 2️⃣ Open in Visual Studio

* Open `.sln` file
* Restore NuGet packages

### 3️⃣ Configure Database

* Update `Web.config` connection string:

```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Server=YOUR_SERVER;Database=VISTARA_DB;Trusted_Connection=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 4️⃣ Run the application

* Press `F5` or click **Run**

---

## 🔐 Authentication

* Roles supported:

  * **MANAGER**
  * **EMPLOYEE**
* Passwords are securely stored using hashing (recommended)

---

## 📦 Project Structure

```
Vistara-Airlines/
│
├── Controllers/
├── Models/
├── Views/
├── Content/
├── Scripts/
├── App_Start/
├── Web.config
└── README.md
```

---

## 🚀 Deployment

This project can be deployed on:

* Microsoft Azure App Service (recommended)
* IIS (Windows Server)

---

## 🎯 Future Enhancements

* Online payment integration
* Real-time seat locking
* Email notifications
* Admin dashboard with analytics
* API integration (RESTful services)

---

## 🤝 Contributing

Contributions are welcome! Feel free to fork the repo and submit pull requests.

---

## 📧 Contact

**Kirankumar Matham**

* GitHub: https://github.com/kirankumar-Matham96
* Email: [mathamkirankumar96@gmail.com](mailto:mathamkirankumar96@gmail.com)

---

## ⭐ Acknowledgements

This project was built as part of learning and implementing **real-world full-stack development practices** using ASP.NET MVC and SQL Server.

---

⭐ If you found this project useful, give it a star!
