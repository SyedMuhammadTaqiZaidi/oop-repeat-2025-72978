using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Web.ViewModels
{
    public class CarCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public required string RegistrationNumber { get; set; }

        [Required]
        public required string CustomerId { get; set; }
    }
}
