using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Web.ViewModels
{
    public class ServiceRecordCreateViewModel
    {
        [Required]
        public required string CustomerId { get; set; }
        [Required]
        public int CarId { get; set; }
        [Required]
        public required string MechanicId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [Range(0, 100)]
        public int HoursWorked { get; set; }
    }
}
