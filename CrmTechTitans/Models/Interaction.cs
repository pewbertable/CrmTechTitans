using CrmTechTitans.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Interaction
    {
        public int Id { get; set; }

        [Display(Name = "Interaction")]
        [Required(ErrorMessage = "Interaction details are required")]
        public string? InteractionDetails { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Display(Name = "Person")]
        public int? ContactId { get; set; } // Foreign key
        public Contact? Contact { get; set; } // Navigation property

        public ICollection<InteractionMember> InteractionMembers { get; set; } = new HashSet<InteractionMember>();
    }
}
