# Project Management API

A comprehensive .NET 8 REST API for project management with advanced team collaboration features.

## Overview

This API provides a complete project management solution built with:
- **Framework**: ASP.NET Core 8
- **Database**: Entity Framework Core 8 with SQL Server
- **Authentication**: JWT Bearer with token rotation
- **Architecture**: Layered architecture (Core → Infrastructure → Services → API)

## Key Features

- 🔐 **JWT Authentication** with secure token rotation and reuse detection
- 👥 **Team Management** with role-based access control
- 📋 **Project Lifecycle Management** (Planning → Active → Completed)
- 🏃 **Sprint Planning** with date-based sprint schedules
- ✅ **Task Management** with status transitions, priority levels, and assignments
- 💬 **Threaded Comments** with nested reply support
- 📎 **File Attachments** with size tracking
- 🔄 **Task History** for audit trails
- 📪 **Notifications** with configurable routing
- 🏷️ **Custom Labels** for task categorization
- 🔑 **Role-Based Access Control** (Admin, Manager, Developer, Viewer)

## Prerequisites

- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download))
- **SQL Server** (LocalDB or full instance)
- **Entity Framework CLI** (install with: `dotnet tool install --global dotnet-ef`)

## Quick Start

### 1. Clone & Install Dependencies

```bash
git clone <repository-url>
cd ProjectManagementAPI
dotnet restore
```

### 2. Configure Database Connection

Edit `appsettings.Development.json` to set your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProjectManagementDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**Default connection strings:**
- **Development**: `Server=(localdb)\\mssqllocaldb;Database=ProjectManagementDb_Dev;Trusted_Connection=true`
- **Production**: Configure in environment-specific `appsettings.{Environment}.json`

### 3. Configure JWT Settings

Update `appsettings.json` with JWT configuration:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-must-be-at-least-32-characters-long",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Security Note**: In production, use Azure Key Vault or environment variables for the secret key:

```bash
# Set as environment variable
$env:JWT_SECRET_KEY = "your-production-secret-key"
```

### 4. Apply Database Migrations

```bash
cd ProjectManagement.API
dotnet ef database update --project ../ProjectManagement.Infrastructure
```

This creates all 18 database tables with proper relationships and constraints.

### 5. Run the API

```bash
dotnet run --project ProjectManagement.API
```

The API will start on `https://localhost:5001`

**API Documentation**: Open `https://localhost:5001` in your browser to view the Swagger UI

## Architecture

### Project Structure

```
ProjectManagementAPI/
├── ProjectManagement.Core/              # Domain layer (no dependencies)
│   ├── Entities/                        # Database entities (18 tables)
│   ├── Enums/                           # Enum definitions (TaskStatus, Priority, etc.)
│   ├── DTOs/                            # Request/Response DTOs
│   ├── Exceptions/                      # Custom exception types
│   └── Interfaces/                      # Contracts for services & repositories
├── ProjectManagement.Infrastructure/    # Data access layer (depends on Core)
│   ├── Persistence/
│   │   ├── AppDbContext.cs              # EF DbContext with 18 dbsets
│   │   ├── Migrations/                  # EF migrations
│   │   └── Configurations/              # EF entity configurations
│   └── Repositories/                    # Generic + specialized repositories
├── ProjectManagement.Services/          # Business logic layer (depends on Core)
│   ├── Services/                        # Service implementations
│   ├── Validators/                      # FluentValidation validators
│   ├── Mappings/                        # AutoMapper profiles
│   └── Extensions/                      # Extension methods
└── ProjectManagement.API/               # REST API layer (depends on all three)
    ├── Controllers/                     # API endpoints (to be created)
    ├── Middleware/                      # Exception handling, logging
    ├── Program.cs                       # DI configuration
    ├── appsettings.json                 # Configuration
    └── Properties/                      # Launch settings
```

### Dependency Flow

```
API → Services → Infrastructure → Core ← (interfaces only)
```

**Layering Rules:**
- ✅ Core: Zero external project references
- ✅ Infrastructure: Only depends on Core
- ✅ Services: Only depends on Core
- ✅ API: Can depend on all three layers

## Database Schema

### 18 Entity Tables

**Authentication (3 tables)**
- `AspNetUsers` – User profiles
- `AspNetRoles` – Role definitions
- `RefreshTokens` – Token rotation tracking

**Teams (2 tables)**
- `Teams` – Team definitions
- `TeamMembers` – Team membership with roles

**Projects (2 tables)**
- `Projects` – Project definitions
- `Sprints` – Sprint planning within projects

**Tasks (5 tables)**
- `Tasks` – Task definitions (renamed from Task to avoid SQL keyword)
- `Comments` – Threaded comments on tasks
- `TaskLabels` – Many-to-many task-label relationships
- `Labels` – Category labels
- `TaskAttachments` – File attachments

**Audit (2 tables)**
- `TaskHistories` – Complete audit trail of changes
- `Notifications` – User notifications

**Identity Framework (4 tables)**
- `AspNetRoleClaims` – Role claims
- `AspNetUserClaims` – User claims
- `AspNetUserLogins` – External login providers
- `AspNetUserTokens` – External provider tokens

### Key Design Decisions

1. **Soft Deletes**: All entities have `IsDeleted` with global query filters
2. **Audit Trail**: All entities track `CreatedAt` and `UpdatedAt`
3. **Token Rotation**: Refresh tokens are hashed with reuse detection
4. **Task Hierarchy**: Tasks support parent-child relationships for subtasks
5. **Status Workflows**: Enums enforce valid state transitions
6. **Cascading Rules**: Strategic use of cascade/restrict for referential integrity

## API Endpoints

### Authentication

```bash
# Register new user
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}

# Login
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}

# Response
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "guid-string",
  "expiresIn": 3600
}

# Refresh Token
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "guid-string"
}

# Logout (revoke token)
POST /api/auth/logout
Authorization: Bearer <accessToken>
```

### Teams

```bash
# Create team
POST /api/teams
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "name": "Development Team",
  "description": "Backend development team"
}

# Get teams
GET /api/teams
Authorization: Bearer <accessToken>

# Add team member
POST /api/teams/{teamId}/members
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "email": "developer@example.com",
  "role": "member"  # or "owner"
}
```

### Projects

```bash
# Create project in team
POST /api/projects
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "teamId": "guid",
  "name": "E-Commerce Platform",
  "description": "Next-gen e-commerce system",
  "key": "ECOM"  # 3-10 chars, unique per team
}

# Get project
GET /api/projects/{projectId}
Authorization: Bearer <accessToken>

# Update project status
PATCH /api/projects/{projectId}
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "status": "Active"  # Planning, Active, Completed
}
```

### Sprints

```bash
# Create sprint
POST /api/sprints
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "projectId": "guid",
  "name": "Sprint 1",
  "startDate": "2024-01-08",
  "endDate": "2024-01-22",
  "status": "Planning"  # Planning, Active, Completed
}

# Start sprint
PATCH /api/sprints/{sprintId}/start
Authorization: Bearer <accessToken>

# Complete sprint
PATCH /api/sprints/{sprintId}/complete
Authorization: Bearer <accessToken>
```

### Tasks

```bash
# Create task
POST /api/tasks
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "projectId": "guid",
  "sprintId": "guid",
  "title": "Implement user authentication",
  "description": "Add JWT-based auth with refresh tokens",
  "priority": "High",      # Low, Medium, High, Critical
  "type": "Story",         # Story, Bug, Task, Subtask
  "storyPoints": 8,
  "dueDate": "2024-01-22",
  "assigneeId": "guid"
}

# Update task status
PATCH /api/tasks/{taskId}/status
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "status": "In Progress"  # Todo, In Progress, In Review, Done, Blocked
}

# Add comment
POST /api/tasks/{taskId}/comments
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "content": "@john Can you review this?",
  "parentCommentId": null  # for threaded replies
}

# Upload attachment
POST /api/tasks/{taskId}/attachments
Authorization: Bearer <accessToken>
Content-Type: multipart/form-data

[multipart form with file]
```

### Labels

```bash
# Create label
POST /api/labels
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "projectId": "guid",
  "name": "Database",
  "color": "#FF5733"
}

# Add label to task
POST /api/tasks/{taskId}/labels
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "labelId": "guid"
}
```

### Notifications

```bash
# Get notifications
GET /api/notifications?pageNumber=1&pageSize=10&isRead=false
Authorization: Bearer <accessToken>

# Mark as read
PATCH /api/notifications/{notificationId}/read
Authorization: Bearer <accessToken>
```

## Configuration Guide

### Environment Variables

Recommended for production:

```bash
# JWT Configuration
set JWT_SECRET_KEY=<32+ character secret>
set JWT_EXPIRATION_MINUTES=60
set JWT_REFRESH_TOKEN_DAYS=7

# Database
set DATABASE_CONNECTION_STRING=<sql-server-connection-string>

# Logging
set LOG_LEVEL=Information
```

### Logging Configuration

Logs are written to:
- **Console**: Real-time output during development
- **File**: Rolling daily files in `logs/` directory

Adjust log levels in `appsettings.{Environment}.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

## Development Workflow

### Adding a New Feature

1. **Define Entity** in `ProjectManagement.Core/Entities/`
2. **Add Migrations**:
   ```bash
   dotnet ef migrations add AddMyEntity --project ProjectManagement.Infrastructure --startup-project ProjectManagement.API
   dotnet ef database update --project ProjectManagement.Infrastructure --startup-project ProjectManagement.API
   ```
3. **Create DTOs** in `ProjectManagement.Core/DTOs/`
4. **Create Service** in `ProjectManagement.Services/Services/`
5. **Create Repository** in `ProjectManagement.Infrastructure/Repositories/`
6. **Add AutoMapper Profile** in `ProjectManagement.Services/Mappings/`
7. **Register Services** in `Program.cs` - `builder.Services.AddScoped<IService, Service>`
8. **Create Controller** in `ProjectManagement.API/Controllers/`

### Running Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project ProjectManagement.Infrastructure --startup-project ProjectManagement.API

# Apply migrations
dotnet ef database update --project ProjectManagement.Infrastructure --startup-project ProjectManagement.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project ProjectManagement.Infrastructure --startup-project ProjectManagement.API

# View migration SQL
dotnet ef migrations script --idempotent --project ProjectManagement.Infrastructure --startup-project ProjectManagement.API
```

## Error Handling

The API implements custom exception handling with HTTP status code mapping:

| Exception | HTTP Status |
|-----------|-------------|
| `UnauthorizedException` | 401 Unauthorized |
| `ForbiddenException` | 403 Forbidden |
| `NotFoundException` | 404 Not Found |
| `ConflictException` | 409 Conflict |
| `BusinessRuleViolationException` | 400 Bad Request |
| Unhandled exceptions | 500 Internal Server Error |

All errors return a consistent response format:

```json
{
  "message": "Detailed error description"
}
```

## Performance Optimizations

- **Query Filtering**: `GetFilteredAsync()` methods support pagination, sorting, and status filtering
- **Lazy Loading**: EF Core configured for explicit loading to prevent N+1 queries
- **Indexes**: Strategic indexes on frequently filtered columns (UserId, Status, ProjectId)
- **Pagination**: All list endpoints support `pageNumber`, `pageSize`, and `sort` parameters

## Security Considerations

1. **Password Hashing**: BCrypt.Net-Next with configurable work factor
2. **JWT Validation**: Signature verification, expiration checks, clock skew tolerance
3. **Token Rotation**: Refresh tokens replaced on each use, old tokens revoked
4. **CORS**: Configured for cross-origin requests (adjust for production)
5. **HTTPS**: Enforced via middleware
6. **SQL Injection**: EF Core parameterized queries prevent SQL injection

## Testing

### Unit Testing Strategy

Create tests in `ProjectManagement.Tests/`:

```csharp
[Fact]
public async Task CreateTask_WithValidData_ReturnsTaskResponse()
{
    // Arrange
    var service = new TaskService(_mockRepository, _mockHistoryService, _mockUnitOfWork);
    var request = new CreateTaskRequest { /* ... */ };

    // Act
    var result = await service.CreateAsync(request, userId, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(request.Title, result.Title);
}
```

### Integration Testing

Use `TestContainers` to spin up SQL Server for integration tests:

```csharp
var container = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    .Build();

await container.StartAsync();
var connectionString = container.GetConnectionString();
```

## Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ProjectManagement.API.dll"]
```

```bash
docker build -t project-management-api:latest .
docker run -p 5001:8080 \
  -e "ConnectionStrings__DefaultConnection=Server=sql-server;..." \
  -e "JwtSettings__SecretKey=..." \
  project-management-api:latest
```

### Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: project-management-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: project-management-api
  template:
    metadata:
      labels:
        app: project-management-api
    spec:
      containers:
      - name: api
        image: project-management-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: api-secrets
              key: db-connection
```

## Monitoring

### Structured Logging with Serilog

Logs include contextual information:

```json
{
  "Timestamp": "2024-01-20T10:30:45.1234567Z",
  "Level": "Information",
  "MessageTemplate": "User {UserId} created project {ProjectId}",
  "UserId": "guid",
  "ProjectId": "guid",
  "SourceContext": "ProjectManagement.Services.Services.ProjectService"
}
```

### Key Metrics to Monitor

- **Response Times**: Track by endpoint and method
- **Error Rates**: Watch for spikes in 5xx errors
- **Database Performance**: Monitor query execution times
- **Token Growth**: Refresh token refresh rates indicate user activity
- **Notification Queue**: Track unread notification counts

## Troubleshooting

### Common Issues

**Issue**: `Connection string 'DefaultConnection' not found`
- **Solution**: Verify `appsettings.json` has `ConnectionStrings.DefaultConnection`

**Issue**: `Migrations not applied`
- **Solution**: Run `dotnet ef database update --project ProjectManagement.Infrastructure`

**Issue**: `JWT signature invalid`
- **Solution**: Ensure `JwtSettings.SecretKey` is the same in all environments

**Issue**: `CORS errors when calling from frontend`
- **Solution**: Update CORS policy in `Program.cs` with correct allowed origins

**Issue**: `NullReferenceException in services`
- **Solution**: Verify all dependencies are registered in `Program.cs`

## Next Steps

### Phase 5 Continuation

After core API setup is complete:

1. **Create API Controllers** - REST endpoints for each entity
2. **Implement Business Logic** - Service layer enhancements
3. **Add Batch Operations** - Bulk task updates, mass notifications
4. **Implement Real-time Updates** - SignalR for live notifications
5. **Add Reporting** - Task completion metrics, team velocity
6. **Implement Search** - Full-text search across tasks
7. **Add Advanced Filtering** - Query builder for complex filters
8. **Implement Webhooks** - Event-driven integrations

## License

This project is provided as-is for educational and development purposes.

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review API documentation at `https://localhost:5001/swagger`
3. Check application logs in `logs/api-*.txt`
4. Verify database connectivity with `dotnet ef migrations list`
