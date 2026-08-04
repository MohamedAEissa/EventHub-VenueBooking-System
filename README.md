# 🏛️ EventHub - Event & Venue Management API

**EventHub** is an enterprise-grade RESTful API engineered with **.NET 8** and **C#**. Built from the ground up using **Clean Architecture** principles and the **CQRS (Command Query Responsibility Segregation)** pattern with **MediatR**, it provides a highly maintainable, testable, and scalable architecture for managing venues, halls, and bookings.

---

## 🏗️ Architectural Overview & Layering

The solution follows a strict **Dependency Rule** where inner layers have no knowledge of outer layers. This decoupled structure ensures high modularity and clean separation of concerns.

                ┌─────────────────────────┐
                │      Presentation       │
                │  (Endpoints / Minimal)  │
                └────────────┬────────────┘
                             │
                ┌────────────▼────────────┐
                │       Application       │
                │   (CQRS / MediatR /     │
                │    Pipeline Behaviors)  │
                └────────────┬────────────┘
                             │
                ┌────────────▼────────────┐
                │       Infrastructure    │
                │  (EF Core / DB Context) │
                └────────────┬────────────┘
                             │
                ┌────────────▼────────────┐
                │         Domain          │
                │   (Entities / Enums)    │
                └─────────────────────────┘

### 1️⃣ Domain Layer (Core)
* The central layer containing core business entities (e.g., `Venue`, `Hall`, `Booking`), value objects, enums, and domain logic.
* **Zero External Dependencies:** Completely isolated from database frameworks, UI concerns, or third-party libraries.

### 2️⃣ Application Layer (Use Cases & Business Rules)
Designed around the **Vertical Slice Architecture** within features, ensuring feature isolation and scalability:
* **Feature-Based Modular Structure:** Organized into dedicated feature folders (e.g., `Features/Venues`, `Features/Halls`).
* **Feature Isolation:** Each feature encapsulates its own:
  * **Commands & Queries:** Operations segregated strictly using **CQRS**.
  * **DTOs:** Tailored Request and Response Data Transfer Objects (`ResponseVenueDto`, `ResponseHallDto`) preventing entity exposure.
  * **Handlers:** Implemented using **MediatR** (`IRequestHandler`).
* **Common Sub-layer (`Common/`):** Contains reusable abstractions, shared contracts (`IApplicationDbContext`), and generic utilities across all application features.
* **MediatR Pipeline Behaviors:** Intercepts incoming requests to execute cross-cutting concerns (Validation, Logging, Performance Monitoring) transparently before reaching the handler.

### 3️⃣ Infrastructure Layer (External Services & Persistence)
* Handles database communication via **Entity Framework Core**.
* Implements `IApplicationDbContext` interface from the Application layer to execute operations on **SQL Server**.
* Configured with optimized **Fluent API** mappings and query optimizations (`AsNoTracking()`).

### 4️⃣ Presentation / Web API Layer (Clean Endpoints)
* **Modular Endpoints Structure:** Controllers/Endpoints are completely segregated by feature, keeping API route definitions thin, clean, and dedicated to single responsibilities.
* **Clean `Program.cs`:** Avoids messy dependency registration by utilizing extension methods (e.g., `AddApplicationServices()`, `AddInfrastructureServices()`). The entry point remains lean and readable.

---

## ⚡ Performance Optimization & Caching Strategy

To deliver lightning-fast responses on high-read endpoints, the application implements an in-memory caching mechanism:

- **In-Memory Caching (`IMemoryCache`):** Applied to read-heavy query handlers (`GetVenuesQueryHandler`, `GetHallByIdQueryHandler`).
- **Sliding & Absolute Expiration:**
  - **Absolute Expiration (5 mins):** Forces periodic updates to guarantee data freshness and prevent stale content.
  - **Sliding Expiration (1 min):** Automatically cleans up unused cache entries to optimize server memory (RAM).
- **Cache Invalidation:** Integrated directly into Command handlers (`Create`, `Update`, `Delete`) to clear affected keys (`_cache.Remove()`) immediately upon data changes, guaranteeing consistency.

---

## 🛠️ Tech Stack & Key Libraries

- **Framework:** .NET 8 Web API
- **Language:** C#
- **Architecture:** Clean Architecture, CQRS, Vertical Slice Strategy
- **Design Patterns:** Mediator Pattern (MediatR), Pipeline Pattern, Dependency Injection
- **Database & ORM:** SQL Server, Entity Framework Core
- **Caching:** `Microsoft.Extensions.Caching.Memory`
- **Security & Auth:** JWT (JSON Web Tokens) Authentication & Authorization

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


