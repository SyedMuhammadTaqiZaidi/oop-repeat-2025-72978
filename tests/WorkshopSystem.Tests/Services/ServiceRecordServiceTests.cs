using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Application.Interfaces;
using WorkshopSystem.Core.Domain.Entities;
using WorkshopSystem.Core.Domain.Enums;
using WorkshopSystem.Infrastructure.Data;
using WorkshopSystem.Infrastructure.Services;
using Xunit;

namespace WorkshopSystem.Tests.Services
{
    public class ServiceRecordServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IUserService> _mockUserService;
        private readonly ServiceRecordService _service;
        private const decimal HourlyRate = 75.00m;
        private const decimal TaxRate = 0.23m;

        public ServiceRecordServiceTests()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockUserService = new Mock<IUserService>();
            
            // Mock user service to return true for role checks
            _mockUserService.Setup(x => x.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _service = new ServiceRecordService(_context);
        }

        [Fact]
        public async Task CalculateServiceCost_WithValidHours_ReturnsCorrectAmount()
        {
            // Arrange
            var record = new ServiceRecord
            {
                HoursWorked = 2.5m, // 3 hours when rounded up
                Status = ServiceStatus.InProgress
            };
            
            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();

            // Act
            var cost = await _service.CalculateServiceCost(record.Id);
            var expectedCost = 3 * HourlyRate * (1 + TaxRate);

            // Assert
            Assert.Equal(expectedCost, cost);
        }

        [Fact]
        public async Task UpdateServiceRecordStatus_WhenCompleted_SetsCompletionDate()
        {
            // Arrange
            var record = new ServiceRecord
            {
                HoursWorked = 2,
                Status = ServiceStatus.InProgress,
                AssignedMechanicId = "mechanic1"
            };
            
            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.UpdateServiceRecordStatusAsync(
                record.Id, ServiceStatus.Completed, "mechanic1");
            var updatedRecord = await _context.ServiceRecords.FindAsync(record.Id);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedRecord.CompletionDate);
            Assert.Equal(ServiceStatus.Completed, updatedRecord.Status);
            Assert.True(updatedRecord.ServiceCost > 0);
        }

        [Fact]
        public async Task AssignMechanic_WithValidMechanic_UpdatesRecord()
        {
            // Arrange
            var record = new ServiceRecord { Status = ServiceStatus.Pending };
            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();

            const string mechanicId = "mechanic123";
            _mockUserService.Setup(x => x.IsInRoleAsync(mechanicId, "Mechanic"))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AssignMechanicAsync(record.Id, mechanicId);
            var updatedRecord = await _context.ServiceRecords.FindAsync(record.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(mechanicId, updatedRecord.AssignedMechanicId);
            Assert.Equal(ServiceStatus.InProgress, updatedRecord.Status);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
