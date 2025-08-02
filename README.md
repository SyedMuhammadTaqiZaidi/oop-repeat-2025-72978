

## 🌟 Features

### 👥 **Role-Based Access Control**
- **Admin**: Full system access - manage customers, cars, service records
- **Mechanic**: Edit assigned service records (description, hours, status)
- **Customer**: View their own service records




### 📊 **Database Features**
- MySQL database with Entity Framework Core
- Automatic database seeding with sample data
- User authentication with ASP.NET Core Identity

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- MySQL 8.4


### Installation

1. **Clone the repository**
   ```bash
   git clone <https://github.com/SyedMuhammadTaqiZaidi/oop-repeat-2025-72978.git>
  
   ```

2. **Configure Database**
   - Update connection string in `appsettings.json`
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=WorkshopSystem;User=root;Password=your_password;"
     }
   }
   ```

3. **Build and Run**
   ```bash
   dotnet restore src/WorkshopSystem.Web/WorkshopSystem.Web.csproj
   dotnet build src/WorkshopSystem.Web/WorkshopSystem.Web.csproj
  App Run CMD :    dotnet run --project src/WorkshopSystem.Web/WorkshopSystem.Web.csproj
  API Run CMD :    dotnet run --project src/WorkshopSystem.API/WorkshopSystem.API.csproj
  Test Run CMD:    dotnet test src/WorkshopSystem.Tests/WorkshopSystem.Tests.csproj
   ```

4. **Access Application**
   - Open browser: http://localhost:(Port Show in terminal)
   - Login with demo credentials (shown on login page)

## 👤 Demo Credentials

Emails:
admin@carservice.com
customer1@carservice.com
customer2@carservice.com
mechanic1@carservice.com
mechanic2@carservice.com


All users Password
Dorset001^

## 🏗️ Project Structure

```
WorkshopSystem/
├── src/
│   ├── WorkshopSystem.Core/           # Domain entities and interfaces
│   ├── WorkshopSystem.Infrastructure/ # Data access and services
│   ├── WorkshopSystem.Web/           # ASP.NET Core MVC application
│   └── WorkshopSystem.API/           # REST API (optional)
├── tests/
│   └── WorkshopSystem.Tests/         # xUnit test project
└── README.md
```

## 🔧 Core Components

### **Controllers**
- `ServiceRecordsController` - Service record management
- `CarsController` - Car management (Admin only)
- `CustomersController` - Customer management (Admin only)
- `AccountController` - Authentication

### **Services**
- `ServiceRecordService` - Business logic for service records
- `UserService` - User management and authentication

### **Models & DTOs**
- `ServiceRecordDto` - Service record data transfer
- `UserDto` - User information
- `CreateServiceRecordDto` - Service record creation

## 🎯 Key Features

### **Service Record Workflow**
1. **Admin creates service record** (customer, car, mechanic assignment)
2. **Mechanic updates** (description, hours worked, status)
3. **Automatic cost calculation** (€75/hour, rounded up)
4. **Service completion** with status tracking

### **Role-Based Permissions**
- **Admin**: Full CRUD operations on all entities
- **Mechanic**: Edit assigned service records only
- **Customer**: View own service records only

### **UI Features**
- Responsive navigation with role-based visibility
- Professional login page with demo credentials
- Clean, modern interface with green-purple theme
- Form validation and error handling

## 🧪 Testing

Run xUnit tests:
```bash
dotnet test tests/WorkshopSystem.Tests/WorkshopSystem.Tests.csproj
```

### Test Coverage
- Service record DTO validation
- User DTO validation
- Service cost calculation
- Service status enum validation

## 🛠️ Development

### **Adding New Features**
1. Create DTOs in `WorkshopSystem.Core/Application/DTOs`
2. Implement services in `WorkshopSystem.Infrastructure/Services`
3. Add controllers in `WorkshopSystem.Web/Controllers`
4. Create views in `WorkshopSystem.Web/Views`

### **Database Migrations**
```bash
dotnet ef migrations add MigrationName --project src/WorkshopSystem.Infrastructure
dotnet ef database update --project src/WorkshopSystem.Infrastructure
```



## 🔒 Security Features

- ASP.NET Core Identity for authentication
- Role-based authorization
- Anti-forgery token validation
- Secure password hashing
- Session management




1. **Port already in use**
   ```bash
   Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
   ```

2. **Database connection issues**
   - Verify MySQL server is running
   - Check connection string in `appsettings.json`
   - Ensure database exists

3. **Build errors**
   ```bash
   dotnet clean
   dotnet build
   ```


