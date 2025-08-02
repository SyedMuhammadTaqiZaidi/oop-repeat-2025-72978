using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Application.Interfaces;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRecordsController : ControllerBase
    {
        private readonly IServiceRecordService _serviceRecordService;
        private readonly ILogger<ServiceRecordsController> _logger;

        public ServiceRecordsController(IServiceRecordService serviceRecordService, ILogger<ServiceRecordsController> logger)
        {
            _serviceRecordService = serviceRecordService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRecordDto>>> GetServiceRecords()
        {
            var records = await _serviceRecordService.GetAllServiceRecordsAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRecordDto>> GetServiceRecord(int id)
        {
            var record = await _serviceRecordService.GetServiceRecordByIdAsync(id);
            return record == null ? NotFound() : Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRecordDto>> CreateServiceRecord(CreateServiceRecordDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdRecord = await _serviceRecordService.CreateServiceRecordAsync(createDto);
            return CreatedAtAction(nameof(GetServiceRecord), new { id = createdRecord.Id }, createdRecord);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRecord(int id, UpdateServiceRecordDto updateDto)
        {
            if (id != updateDto.Id) return BadRequest("ID mismatch");
            await _serviceRecordService.UpdateServiceRecordAsync(id, updateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRecord(int id)
        {
            await _serviceRecordService.DeleteServiceRecordAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/assign/{mechanicId}")]
        public async Task<IActionResult> AssignMechanic(int id, string mechanicId)
        {
            var success = await _serviceRecordService.AssignMechanicAsync(id, mechanicId);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteService(int id)
        {
            var updatedBy = User.Identity?.Name ?? "Unknown";
            var success = await _serviceRecordService.UpdateServiceRecordStatusAsync(
                id, ServiceStatus.Completed, updatedBy);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("{id}/cost")]
        public async Task<ActionResult<decimal>> CalculateServiceCost(int id)
        {
            var cost = await _serviceRecordService.CalculateServiceCost(id);
            return Ok(new { TotalCost = cost });
        }
    }
}
