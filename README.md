# JobTracker

JobTracker is a **full-stack job application tracker** built with .NET 9, Entity Framework Core, and JWT-based authentication. It allows users to register, login, and manage their job applications securely.

---

## Table of Contents

- [Technologies](#technologies)
- [Features](#features)
- [Getting Started](#getting-started)
- [Environment & Secrets Setup](#environment--secrets-setup)
- [Running the Project](#running-the-project)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Contributing](#contributing)

---

## Technologies

- **Backend**: .NET 9, C#
- **Database**: PostgreSQL, Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Dependency Injection**: Built-in .NET DI
- **Testing**: xUnit / Integration Tests
- **API Docs**: Swagger

---

## Features

- User registration and login with secure password hashing (BCrypt)
- JWT authentication and authorization
- User roles (basic role system implemented)
- CRUD operations for Job Applications (to be implemented)
- CORS configured for development and production
- Swagger UI for API documentation

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- Optional: [Docker](https://www.docker.com/) (if you want containerized DB)

### Setup

1. **Clone the repository**

```bash
git clone https://github.com/yourusername/JobTracker.git
cd JobTracker
```

2. **Install dependencies**

```bash
dotnet restore
```

3. **Run migrations**

```bash
dotnet ef database update --project src/JobTracker.Infrastructure --startup-project src/JobTracker.Api
```

---

## Environment & Secrets Setup

To keep secrets like database credentials and JWT keys safe, we use **environment variables** or `appsettings.{Environment}.json` files.

### Example `appsettings.Development.json`

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Host=localhost;Database=JobTrackerDb;Username=postgres;Password=yourpassword"
	},
	"Jwt": {
		"Key": "your-super-secret-key",
		"Issuer": "JobTrackerApi",
		"Audience": "JobTrackerClient"
	}
}
```

### Environment Variables Alternative

Set these in your system or in a `.env` file (if using a secrets manager):

```bash
# PostgreSQL
export POSTGRES_HOST=localhost
export POSTGRES_DB=JobTrackerDb
export POSTGRES_USER=postgres
export POSTGRES_PASSWORD=yourpassword

# JWT
export JWT_KEY=your-super-secret-key
export JWT_ISSUER=JobTrackerApi
export JWT_AUDIENCE=JobTrackerClient
```

Then, in `Program.cs` or `builder.Configuration`, you can read:

```csharp
builder.Configuration["Jwt:Key"]
builder.Configuration.GetConnectionString("DefaultConnection")
```

> Using environment variables ensures that secrets are not committed to the repository.

---

## Running the Project

```bash
cd src/JobTracker.Api
dotnet run
```

- Swagger UI: `http://localhost:5000/swagger`
- API is ready for JWT-authenticated requests.

---

## API Endpoints

### Authentication

| Method | Endpoint             | Description                          |
| ------ | -------------------- | ------------------------------------ |
| POST   | `/api/auth/register` | Registers a new user                 |
| POST   | `/api/auth/login`    | Logs in a user and returns JWT token |

### Job Applications (planned)

| Method | Endpoint                 | Description                                  |
| ------ | ------------------------ | -------------------------------------------- |
| GET    | `/api/applications`      | List all applications for authenticated user |
| GET    | `/api/applications/{id}` | Get details of a specific application        |
| POST   | `/api/applications`      | Create a new job application                 |
| PUT    | `/api/applications/{id}` | Update an existing application               |
| DELETE | `/api/applications/{id}` | Delete an application                        |

> All Job Applications endpoints require authentication with a valid JWT.

---

## Project Structure

```
JobTracker/
│
├─ src/
│  ├─ JobTracker.Api/            # API project
│  ├─ JobTracker.Application/    # DTOs, Interfaces, Service contracts
│  ├─ JobTracker.Domain/         # Entities, common models
│  ├─ JobTracker.Infrastructure/ # Persistence, Repositories, Services implementation
│
├─ tests/
│  ├─ JobTracker.IntegrationTests/ # Integration tests
│
├─ README.md
├─ .gitignore
└─ ... other solution files
```

---

## Contributing

1. Fork the repository
2. Create a new branch (`git checkout -b feature/FeatureName`)
3. Commit your changes (`git commit -m "Add feature"`)
4. Push to the branch (`git push origin feature/FeatureName`)
5. Open a Pull Request

---

## License

This project is open-source under the MIT License.

---
