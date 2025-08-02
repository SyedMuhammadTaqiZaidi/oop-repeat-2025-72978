using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.Core.Application.Interfaces
{
    public interface IServiceRecordService
    {
        Task<IEnumerable<ServiceRecordDto>> GetAllServiceRecordsAsync();
        Task<ServiceRecordDto> GetServiceRecordByIdAsync(int id);
        Task<IEnumerable<ServiceRecordDto>> GetServiceRecordsByRequestedByIdAsync(string requestedById);
        Task<IEnumerable<ServiceRecordDto>> GetServiceRecordsByMechanicIdAsync(string mechanicId);
        Task<IEnumerable<ServiceRecordDto>> GetServiceRecordsByCustomerIdAsync(string customerId);
        Task<ServiceRecordDto> CreateServiceRecordAsync(CreateServiceRecordDto createDto);
        Task UpdateServiceRecordAsync(int id, UpdateServiceRecordDto updateDto);
        Task UpdateServiceStatusAsync(int id, ServiceStatus status);
        Task<bool> UpdateServiceRecordStatusAsync(int id, ServiceStatus status, string updatedBy);
        Task<bool> AssignMechanicAsync(int id, string mechanicId);
        Task DeleteServiceRecordAsync(int id);
        Task<decimal> CalculateServiceCost(int id);
    }
}
