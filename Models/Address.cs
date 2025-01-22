using System.ComponentModel.DataAnnotations;
using CrmTechTitans.Models.Enumerations;

namespace CRM.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street address can't be longer than 200 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City name can't be longer than 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public Province Province { get; set; }


        [StringLength(20, ErrorMessage = "ZIP code can't be longer than 20 characters")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZIP code format (should be 5 digits, optionally followed by a hyphen and 4 digits)")]
        public string? Zip { get; set; }
    }
}
