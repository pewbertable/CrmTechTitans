using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }
    }
} 