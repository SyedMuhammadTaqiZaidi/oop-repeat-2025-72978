using System;
using System.ComponentModel.DataAnnotations;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.Core.Application.DTOs
{
    public class ServiceRecordDto : BaseDto
    {
        [Required(ErrorMessage = "Service date is required")]
        public DateTime ServiceDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Service cost is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Service cost must be a positive number")]
        public decimal ServiceCost { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 1000, ErrorMessage = "Hours worked must be between 0 and 1000")]
        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public ServiceStatus Status { get; set; }

       
        [Required(ErrorMessage = "Mechanic ID is required")]
        public string AssignedMechanicId { get; set; }

        [Required(ErrorMessage = "Requested by ID is required")]
        public string RequestedById { get; set; }

        
        public string MechanicName { get; set; }
        public string RequestedByName { get; set; }
        public string CustomerName { get; set; }

        public int CarId { get; set; }
        public string CarRegistrationNumber { get; set; }
    }

    public class CreateServiceRecordDto
    {
        [Required(ErrorMessage = "Customer is required")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "Car is required")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Mechanic ID is required")]
        public string AssignedMechanicId { get; set; }

        [Required(ErrorMessage = "Requested by ID is required")]
        public string RequestedById { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0, 1000, ErrorMessage = "Hours worked must be between 0 and 1000")]
        public int HoursWorked { get; set; }
    }

    
}
