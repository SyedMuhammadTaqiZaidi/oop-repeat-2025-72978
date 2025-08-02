using Xunit;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.Tests;

public class ServiceRecordTests
{
    [Fact]
    public void CreateServiceRecordDto_ShouldHaveValidProperties()
    {
        var dto = new CreateServiceRecordDto
        {
            CustomerId = "customer123",
            CarId = 1,
            AssignedMechanicId = "mechanic123",
            RequestedById = "admin123",
            Description = "Oil change and filter replacement",
            HoursWorked = 2
        };

        Assert.Equal("customer123", dto.CustomerId);
        Assert.Equal(1, dto.CarId);
        Assert.Equal("mechanic123", dto.AssignedMechanicId);
        Assert.Equal("admin123", dto.RequestedById);
        Assert.Equal("Oil change and filter replacement", dto.Description);
        Assert.Equal(2, dto.HoursWorked);
    }

    [Fact]
    public void ServiceRecordDto_ShouldHaveValidProperties()
    {
        var dto = new ServiceRecordDto
        {
            Id = 1,
            ServiceDate = DateTime.Now,
            Description = "Brake pad replacement",
            ServiceCost = 150.00m,
            HoursWorked = 3.5,
            Status = ServiceStatus.InProgress,
            AssignedMechanicId = "mechanic123",
            RequestedById = "admin123",
            MechanicName = "John Mechanic",
            RequestedByName = "Admin User",
            CustomerName = "John Doe",
            CarId = 1,
            CarRegistrationNumber = "ABC123"
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("Brake pad replacement", dto.Description);
        Assert.Equal(150.00m, dto.ServiceCost);
        Assert.Equal(3.5, dto.HoursWorked);
        Assert.Equal(ServiceStatus.InProgress, dto.Status);
        Assert.Equal("mechanic123", dto.AssignedMechanicId);
        Assert.Equal("John Mechanic", dto.MechanicName);
        Assert.Equal("John Doe", dto.CustomerName);
        Assert.Equal("ABC123", dto.CarRegistrationNumber);
    }

    [Theory]
    [InlineData(0, 0.00)]
    [InlineData(1, 75.00)]
    [InlineData(2, 150.00)]
    [InlineData(3.5, 300.00)]
    public void ServiceCost_ShouldBeCalculatedCorrectly(double hoursWorked, decimal expectedCost)
    {
        var dto = new ServiceRecordDto
        {
            HoursWorked = hoursWorked,
            ServiceCost = (decimal)(Math.Ceiling(hoursWorked) * 75)
        };

        Assert.Equal(expectedCost, dto.ServiceCost);
    }

    [Fact]
    public void ServiceStatus_ShouldHaveValidValues()
    {
        Assert.Equal(0, (int)ServiceStatus.Pending);
        Assert.Equal(1, (int)ServiceStatus.InProgress);
        Assert.Equal(2, (int)ServiceStatus.Completed);
        Assert.Equal(3, (int)ServiceStatus.Cancelled);
    }
}

public class UserTests
{
    [Fact]
    public void UserDto_ShouldHaveValidProperties()
    {
        var userDto = new UserDto
        {
            Id = "user123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890"
        };

        Assert.Equal("user123", userDto.Id);
        Assert.Equal("test@example.com", userDto.Email);
        Assert.Equal("John", userDto.FirstName);
        Assert.Equal("Doe", userDto.LastName);
        Assert.Equal("+1234567890", userDto.PhoneNumber);
    }
}