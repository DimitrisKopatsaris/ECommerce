🛒 E-Commerce API
A production-oriented E-Commerce backend API built with ASP.NET Core, focusing on business logic, data consistency, and transactional operations.

This project demonstrates how to model a real e-commerce domain with products, orders, and inventory, using a layered approach and Entity Framework Core.

---

🚀 Features
Product management
Order creation and processing
Stock and inventory handling
Business rules enforcement
Transactional operations with EF Core
Clean separation of concerns
Repository & service-based design
Entity Framework Core migrations
RESTful API design
Swagger / OpenAPI documentation

---

🧱 Project Structure
src/
 ├── ECommerce.Api
 │   ├── Controllers
 │   ├── Program.cs
 │   └── Extensions
 │
 ├── ECommerce.Application
 │   ├── Services
 │   ├── Interfaces
 │   └── DTOs
 │
 ├── ECommerce.Domain
 │   ├── Entities
 │   └── ValueObjects
 │
 └── ECommerce.Infrastructure
     ├── Persistence
     ├── Repositories
     └── Migrations

---

🧠 Architectural Approach
Controllers:
Handle HTTP requests and responses only.
Application layer:
Contains business logic and use cases (orders, stock updates, validations).
Domain layer:
Holds core entities and domain rules.
Infrastructure layer:
Handles database access via Entity Framework Core and repository implementations.
This separation keeps the system maintainable, testable, and scalable as business complexity grows.

---

🛠 Tech Stack
C# / ASP.NET Core
Entity Framework Core
SQL Server
Repository pattern
Transactions
Swagger / OpenAPI
Git

---

▶️ Run Locally
Prerequisites:
.NET SDK 8+
SQL Server (local or Docker)

Steps:
dotnet restore
dotnet ef database update
dotnet run --project src/ECommerce.Api

Swagger UI:
https://localhost:<configured-port>/swagger

---

📌 Example Endpoints
GET    /api/products
POST   /api/products
POST   /api/orders
GET    /api/orders/{id}

---

🧠 What This Project Demonstrates
Modeling real-world business domains
Managing state changes (orders, stock, inventory)
Using transactions to ensure data consistency
Applying clean separation of responsibilities
Writing backend code beyond basic CRUD operations

---

🔮 Future Improvements
Authentication & authorization
Payment integration simulation
Order status lifecycle (Pending, Paid, Shipped)
Concurrency handling for high-traffic scenarios
Integration tests
Docker support

---

📎 Why This Project Exists
This project was built as part of my backend engineering portfolio to demonstrate realistic e-commerce backend design, focusing on business logic, consistency, and maintainability.

---

⭐ Recruiter Note
This repository complements my Auth API and Expense Tracker projects by showcasing transactional business logic and domain modeling in a backend system.