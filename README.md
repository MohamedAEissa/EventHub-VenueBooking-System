# 🏛️ EventHub System (Web API)

A production-ready Enterprise Event & Venue Booking backend built with the latest **.NET 9 Web API**. The project is designed using modern architectural best practices, enforcing strict Clean Architecture layer isolation, type safety, robust validation, high security, and high-performance in-memory caching.

---

## 👤 Developer Profile & Contact

* **Developer:** Mohamed Atef Bayoumi Eissa
* **Title:** Junior .NET Developer
* **Location:** New Cairo, Egypt (Open to Relocate)
* **Phone:** (+20) 1050908329
* **Email:** mohamedeissa123123@gmail.com
* **GitHub:** [MohamedAEissa](https://github.com/MohamedAEissa) 
* **LinkedIn:** [mohamed-atef](https://www.linkedin.com/in/mohamed-atef)

> **Objective:** Junior .NET Developer with hands-on experience building ASP.NET Core web applications using C#, Entity Framework Core, SQL Server, and REST APIs. Passionate about writing clean, maintainable code and eager to contribute to a collaborative engineering team.

---

## 🌟 Core Domain Features

The system is built around a rich domain model catering to comprehensive event management:
* **Venues & Halls Management:** Create and manage venues, including specific halls with their respective capacities and amenities.
* **Events System:** Full lifecycle management for events hosted at specific halls.
* **Booking Engine:** A robust booking mechanism ensuring no double-booking, managing available slots and user reservations.
* **Polls System:** Interactive polling features allowing users/attendees to vote and participate in event-related decisions.
* **Reviews & Ratings:** Users can leave detailed reviews and ratings for venues and events they attended.

---

## 🏗️ Clean Architecture & Layered Design

The solution strictly adheres to **Clean Architecture** principles, ensuring the Dependency Rule is never violated. Inner layers have no knowledge of outer layers.

1. **Domain Layer (Core):** Contains core business Entities, Value Objects, and Enums. Zero external dependencies.
2. **Application Layer:** Contains all business logic organized by **Vertical Slices (Features)**. It holds Interfaces, CQRS Commands/Queries, DTOs, Mapping logic, and FluentValidation rules.
3. **Infrastructure Layer:** Handles external concerns. Contains the EF Core `DbContext`, database migrations, and third-party service implementations.
4. **Presentation Layer (Web API):** Very thin layer containing API Endpoints/Controllers. It delegates all work to the Application layer via MediatR.

---

## ⚙️ Architectural Patterns & Design Choices

* **CQRS Pattern (Command Query Responsibility Segregation):** Segregates read operations (Queries) from write operations (Commands) using **MediatR**, keeping handlers thin, single-purpose, and easily testable.
* **MediatR Pipeline Behaviors:** Intercepts every incoming request to handle cross-cutting concerns (Validation, Logging) transparently before reaching the target handler.
* **Data Transfer Objects (DTOs) & Mapping:** Entities are strictly protected. Incoming and outgoing data is mapped through specific DTOs using Mappers, ensuring sensitive data is never exposed.
* **FluentValidation:** Robust, strongly-typed input validation piped directly through MediatR Behaviors to catch bad requests early.

---

## 🔐 Security & Identity (Access & Refresh Tokens)

The API enforces strict, modern security protocols using Custom JWT Bearer Authentication:
* **Access Tokens:** Short-lived JWT tokens used for stateless, secure API access.
* **Refresh Tokens:** Secure, long-lived tokens stored in the database. They allow users to seamlessly maintain their active sessions without constantly re-entering credentials, greatly enhancing UX and security.
* **Role-Based Authorization (RBAC):** 
  * **Admin / Venue Owner:** Granted full management rights (Creating venues, updating events, managing halls).
  * **User:** Limited to browsing, interacting with polls, submitting reviews, and booking slots.

---

## ⚡ Performance Optimization (Smart Caching)

To deliver lightning-fast response times on high-traffic read endpoints, the system incorporates an enterprise-grade caching strategy:
* **In-Memory Caching (`IMemoryCache`):** Applied to read-heavy Query Handlers.
* **Sliding & Absolute Expiration:** Ensures memory is freed from unused cache (Sliding) while guaranteeing data freshness (Absolute).
* **Smart Cache Invalidation:** Integrated directly into Command Handlers (`Create`, `Update`, `Delete`) to immediately purge affected cache keys upon data mutation, guaranteeing 100% data consistency.

---

## 📊 Logging & Monitoring

* **Structured Logging:** Integrated robust logging mechanisms across the application.
* **Traceability:** Captures critical system events, warnings, database queries, and error stack traces to facilitate fast debugging and ensure the system's health in production environments.

---

## 🛠️ Complete Tech Stack

* **Framework:** `.NET 9.0 (Web API)`
* **Language:** `C#`
* **Architecture:** Clean Architecture + CQRS + Vertical Slice Strategy
* **Design Patterns:** Mediator Pattern, Pipeline Pattern, Repository/DbContext Abstraction
* **ORM / Database:** `Entity Framework Core 9` with **SQL Server**.
* **CQRS & Messaging:** `MediatR` for decoupled in-process messaging.
* **Validation & Mapping:** `FluentValidation` & Auto/Object Mapping.
* **Caching:** `Microsoft.Extensions.Caching.Memory` with dynamic invalidation.
* **Security & Auth:** `Identity`, `JWT Bearer Tokens`, & **Refresh Tokens**.
* **API Documentation:** **`Scalar`** (Modern, beautiful, and interactive API reference replacing traditional Swagger).

---

## 📂 Project Directory Structure

```text
EventHub/
├── src/
│   ├── EventHub.Domain/             # Core Entities (Venues, Halls, Events, Polls, Reviews)
│   │
│   ├── EventHub.Application/        # Business Logic (CQRS, MediatR, DTOs, Validators, Mappers)
│   │
│   ├── EventHub.Infrastructure/     # DbContext, Migrations, External Integrations
│   │
│   └── EventHub.WebApi/             # Endpoints, Middlewares, Scalar Setup, Program.cs
└── README.md

------

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server

### Installation & Execution

1. **Clone the Repository:**
   ```bash
   git clone [https://github.com/MohamedAEissa/EventHub.git](https://github.com/MohamedAEissa/EventHub.git)
   cd EventHub      
2. **Configure Connection String:
   Update the database connection string in src/EventHub.WebApi/appsettings.json:
   "ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EventHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}    
3. **Apply Database Migrations:
    dotnet ef database update --project src/EventHub.Infrastructure --startup-project src/EventHub.WebApi

4. **Run Application:
   dotnet run --project src/EventHub.WebApi


