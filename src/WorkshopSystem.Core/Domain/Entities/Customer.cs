using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkshopSystem.Core.Domain.Entities
{
    public class Customer : ApplicationUser
    {
        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }


    }
}
