using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Application.Interfaces;
using WorkshopSystem.Core.Domain.Entities;
using WorkshopSystem.Core.Domain.Enums;
using WorkshopSystem.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopSystem.Infrastructure.Services
{
    public class ServiceRecordService : IServiceRecordService
    {
        private readonly ApplicationDbContext _context;
        private const decimal HourlyRate = 75.00m;

        public ServiceRecordService(ApplicationDbContext context) => _context = context;

        public async Task<ServiceRecordDto> GetServiceRecordByIdAsync(int id)
        {
            var record = await _context.ServiceRecords
                .Include(sr => sr.AssignedMechanic)
                .Include(sr => sr.RequestedBy)
                .FirstOrDefaultAsync(sr => sr.Id == id);
            
            return record != null ? MapToDto(record) : null;
        }

        public async Task<ServiceRecordDto> CreateServiceRecordAsync(CreateServiceRecordDto dto)
        {
            var hours = (decimal)Math.Ceiling((double)dto.HoursWorked);
            var serviceCost = hours * HourlyRate;

            var record = new ServiceRecord
            {
                ServiceDate = DateTime.UtcNow,
                Description = dto.Description,
                Status = ServiceStatus.Pending,
                AssignedMechanicId = dto.AssignedMechanicId,
                RequestedById = dto.RequestedById,
                CustomerId = dto.CustomerId,
                CarId = dto.CarId,
                HoursWorked = dto.HoursWorked,
                ServiceCost = serviceCost
            };

            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();
            return await GetServiceRecordByIdAsync(record.Id);
        }

        public async Task<IEnumerable<ServiceRecordDto>> GetAllServiceRecordsAsync()
        {
            var records = await _context.ServiceRecords
                .Include(sr => sr.AssignedMechanic)
                .Include(sr => sr.RequestedBy)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Car)
                .ToListAsync();
                
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<ServiceRecordDto>> GetServiceRecordsByRequestedByIdAsync(string requestedById)
        {
            var records = await _context.ServiceRecords
                .Include(sr => sr.AssignedMechanic)
                .Include(sr => sr.RequestedBy)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Car)
                .Where(sr => sr.RequestedById == requestedById)
                .ToListAsync();
                
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<ServiceRecordDto>> GetServiceRecordsByMechanicIdAsync(string mechanicId)
        {
            var records = await _context.ServiceRecords
                .Include(sr => sr.AssignedMechanic)
                .Include(sr => sr.RequestedBy)
                .Include(sr => sr.Customer)
                .Include(sr => sr.Car)
                .Where(sr => sr.AssignedMechanicId == mechanicId)
                .ToListAsync();
                
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<ServiceRecordDto>> GetServiceRecordsByCustomerIdAsync(string customerId)
        {
            var records = await _context.ServiceRecords
                .Include(sr => sr.AssignedMechanic)
                .Include(sr => sr.RequestedBy)
                .Include(sr => sr.Car)
                .Where(sr => sr.CustomerId == customerId)
                .ToListAsync();
                
            return records.Select(MapToDto);
        }

        public async Task<decimal> CalculateServiceCost(int id)
        {
            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return 0;
            
            var hours = (decimal)Math.Ceiling((double)record.HoursWorked);
            return hours * HourlyRate;
        }

        public async Task UpdateServiceRecordAsync(int id, UpdateServiceRecordDto updateDto)
        {
            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return;
            
            if (updateDto.Description != null)
                record.Description = updateDto.Description;
                
            if (updateDto.HoursWorked.HasValue)
            {
                record.HoursWorked = updateDto.HoursWorked.Value;
                var hours = (decimal)Math.Ceiling((double)record.HoursWorked);
                record.ServiceCost = hours * HourlyRate;
            }
                
            if (updateDto.Cost.HasValue)
                record.ServiceCost = updateDto.Cost.Value;
                
            if (updateDto.Status.HasValue)
                record.Status = updateDto.Status.Value;
            
            if (!string.IsNullOrEmpty(updateDto.AssignedMechanicId))
                record.AssignedMechanicId = updateDto.AssignedMechanicId;
                
            if (updateDto.Status == ServiceStatus.Completed && !record.CompletionDate.HasValue)
                record.CompletionDate = DateTime.UtcNow;
                
            await _context.SaveChangesAsync();
        }

        public async Task UpdateServiceStatusAsync(int id, ServiceStatus status)
        {
            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return;
            
            record.Status = status;
            
            if (status == ServiceStatus.Completed && !record.CompletionDate.HasValue)
                record.CompletionDate = DateTime.UtcNow;
                
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateServiceRecordStatusAsync(int id, ServiceStatus status, string updatedBy)
        {
            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return false;
            
            record.Status = status;
            
            if (status == ServiceStatus.Completed && !record.CompletionDate.HasValue)
                record.CompletionDate = DateTime.UtcNow;
                
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignMechanicAsync(int id, string mechanicId)
        {
            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return false;
            
            var mechanicExists = await _context.Users.AnyAsync(u => u.Id == mechanicId);
            if (!mechanicExists) return false;
            
            record.AssignedMechanicId = mechanicId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteServiceRecordAsync(int id)
        {
            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return;
            
            _context.ServiceRecords.Remove(record);
            await _context.SaveChangesAsync();
        }

        private static ServiceRecordDto MapToDto(ServiceRecord sr) => new()
        {
            Id = sr.Id,
            ServiceDate = sr.ServiceDate,
            CompletionDate = sr.CompletionDate,
            Description = sr.Description,
            ServiceCost = sr.ServiceCost,
            HoursWorked = sr.HoursWorked,
            Status = sr.Status,
            AssignedMechanicId = sr.AssignedMechanicId,
            RequestedById = sr.RequestedById,
            MechanicName = $"{sr.AssignedMechanic?.FirstName} {sr.AssignedMechanic?.LastName}",
            RequestedByName = $"{sr.RequestedBy?.FirstName} {sr.RequestedBy?.LastName}",
            CustomerName = $"{sr.Customer?.FirstName} {sr.Customer?.LastName}",
            CarId = sr.CarId,
            CarRegistrationNumber = sr.Car?.RegistrationNumber
        };
    }
}
