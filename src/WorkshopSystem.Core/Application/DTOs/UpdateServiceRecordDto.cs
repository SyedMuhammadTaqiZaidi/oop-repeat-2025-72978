using System;
using System.ComponentModel.DataAnnotations;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.Core.Application.DTOs
{
    public class UpdateServiceRecordDto
    {
        [Required]
        public int Id { get; set; }
        public string Description { get; set; }
        public double? HoursWorked { get; set; }
        public decimal? Cost { get; set; }
        public ServiceStatus? Status { get; set; }
        public string AssignedMechanicId { get; set; }
    }
}
