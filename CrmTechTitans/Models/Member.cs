using CrmTechTitans.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Member")]
        [Required(ErrorMessage = "Member is required")]
        [StringLength(100, ErrorMessage = "Member name can't be longer than 100 characters")]
        public string MemberName { get; set; }

        [Required]
        [Display(Name = "Membership Type")]
        public MembershipType MembershipType { get; set; }

        [Display(Name = "Contacted By")]
        [StringLength(100)]
        public string? ContactedBy { get; set; }

        [Required]
        [Display(Name = "Industry Type")]
        public NaicsIndustryCode Industry { get; set; }  // Changed to enum

        [Required]
        [Display(Name = "Company Size")]
        public CompanySize CompanySize { get; set; } // Changed to enum

        [Display(Name = "Website")]
        [Url]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Contact Date")]
        public DateTime? LastContactDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Membership Status")]
        public MembershipStatus MembershipStatus { get; set; }


    }
}
