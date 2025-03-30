using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.Enumerations
{
    public enum MembershipStatus
    {
        [Display(Name = "Good Standing")] 
        GoodStanding ,
      [Display(Name = "Out Standing")]
        OutStanding
            ,
        [Display(Name = "Archived")] 
        Cancelled
    }
}
