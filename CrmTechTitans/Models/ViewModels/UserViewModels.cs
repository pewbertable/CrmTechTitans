using CrmTechTitans.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.ViewModels
{
    /// <summary>
    /// View model for displaying user information in a list
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public UserApprovalStatus ApprovalStatus { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool RegistrationComplete { get; set; }
    }

    /// <summary>
    /// View model for user profile
    /// </summary>
    public class ProfileViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }
    }

    /// <summary>
    /// View model for creating a new user
    /// </summary>
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string SelectedRole { get; set; } = string.Empty;
        
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }

    /// <summary>
    /// View model for editing a user
    /// </summary>
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string SelectedRole { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Approval Status")]
        public UserApprovalStatus ApprovalStatus { get; set; }
        
        [Display(Name = "Rejection Reason")]
        public string? RejectionReason { get; set; }
        
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
} 