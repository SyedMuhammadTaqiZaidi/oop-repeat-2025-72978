using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Core.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

       
        public virtual ICollection<ServiceRecord> ServiceRequests { get; set; } = new List<ServiceRecord>();
        public virtual ICollection<ServiceRecord> ServicesPerformed { get; set; } = new List<ServiceRecord>();
    }
}
