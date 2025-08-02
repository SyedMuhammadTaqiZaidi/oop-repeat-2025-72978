using System;
using System.ComponentModel.DataAnnotations;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.Core.Domain.Entities
{
    public class ServiceRecord
    {
        public int Id { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ServiceCost { get; set; }

        [Range(0, 1000)]
        public double HoursWorked { get; set; }

        [Required]
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

      
        public string CustomerId { get; set; }
        public int CarId { get; set; }
        public string AssignedMechanicId { get; set; }
        public string RequestedById { get; set; }

       
        public virtual ApplicationUser Customer { get; set; }
        public virtual Car Car { get; set; }
        public virtual ApplicationUser AssignedMechanic { get; set; }
        public virtual ApplicationUser RequestedBy { get; set; }
    }
}
