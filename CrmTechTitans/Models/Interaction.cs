using CrmTechTitans.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Interaction
    {
        public int Id { get; set; }

        [Display(Name = "Interaction")]
        public string? interaction { get; set; }


        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Person")]
        public string? Person { get; set; }




        public ICollection<InteractionMember> InteractionMembers { get; set; } = new HashSet<InteractionMember>();

    }
}
