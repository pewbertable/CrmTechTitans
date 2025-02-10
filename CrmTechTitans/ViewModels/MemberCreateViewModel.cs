using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.ViewModels
{
    public class MemberCreateViewModel
    {
        // Member Properties
        public int ID { get; set; }

        [Display(Name = "Member Name")]
        [Required(ErrorMessage = "Member Name is required")]
        [StringLength(100, ErrorMessage = "Member Name cannot exceed 100 characters")]
        public string MemberName { get; set; }

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "Membership Type is required")]
        public int SelectedMembershipTypeID { get; set; } // Stores the selected MembershipType ID

        public List<MembershipTypeViewModel> AvailableMembershipTypes { get; set; } = new List<MembershipTypeViewModel>(); // List for dropdown


        [Display(Name = "Contacted By")]
        [StringLength(100, ErrorMessage = "Contacted By cannot exceed 100 characters")]
        public string ContactedBy { get; set; }

        [Display(Name = "Company Size")]
        [Required(ErrorMessage = "Company Size is required")]
        public CompanySize CompanySize { get; set; }

        [Display(Name = "Company Website")]
        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(255, ErrorMessage = "Website cannot exceed 255 characters")]
        public string CompanyWebsite { get; set; }

        [Display(Name = "Member Since")]
        [DataType(DataType.Date)]
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Contact Date")]
        [DataType(DataType.Date)]
        public DateTime? LastContactDate { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [Display(Name = "Membership Status")]
        [Required(ErrorMessage = "Membership Status is required")]
        public MembershipStatus MembershipStatus { get; set; }

        public MemberPhoto? MemberPhoto { get; set; }

        public MemberThumbnail? MemberThumbnail { get; set; }

        // Address Properties
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();

        // Contacts Properties
        public List<ContactViewModel> Contacts { get; set; } = new List<ContactViewModel>();

        // Industries Properties
        [Display(Name = "Industries")]
        public List<int> SelectedIndustryIds { get; set; } = new List<int>();

        public List<IndustryViewModel> AvailableIndustries { get; set; } = new List<IndustryViewModel>(); // ✅ FIXED
    }

    public class AddressViewModel
    {
        public int ID { get; set; } // ✅ Added for tracking edits

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Display(Name = "Province")]
        [Required(ErrorMessage = "Province is required")]
        public Province Province { get; set; }

        [Display(Name = "Postal Code")]
        [StringLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ ]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code format (A#A #A#)")]
        public string PostalCode { get; set; }

        [Display(Name = "Address Type")]
        [Required(ErrorMessage = "Address Type is required")]
        public AddressType AddressType { get; set; }
    }

    public class ContactViewModel
    {
        public int ID { get; set; } // ✅ Added for tracking edits

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string Phone { get; set; }

        [Display(Name = "Contact Type")]
        [Required(ErrorMessage = "Contact Type is required")]
        public ContactType ContactType { get; set; }

        public ContactPhoto? ContactPhoto { get; set; }

        public ContactThumbnail? ContactThumbnail { get; set; }
    }

    public class IndustryViewModel
    {
        public int ID { get; set; } // ✅ Added for binding in views

        [Display(Name = "Industry")]
        [Required(ErrorMessage = "Industry is required")]
        [StringLength(100, ErrorMessage = "Industry cannot exceed 100 characters")]
        public string Name { get; set; }

        [Display(Name = "NAICS Code")]
        [Required(ErrorMessage = "NAICS Code is required")]
        public int NAICS { get; set; }
    }

  
     public class MembershipTypeViewModel
      {
                public int ID { get; set; }

                [Display(Name = "Membership Type")]
                public string Name { get; set; }
      }
    

}
