using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.Enumerations
{
    public enum Province
    {
        Alberta,
        [Display(Name = "British Columbia")]
        BritishColumbia,
        Manitoba,
        [Display(Name = "New Brunswick")]
        NewBrunswick,
        [Display(Name = "Newfoundland And Labrador")]
        NewfoundlandAndLabrador,
        [Display(Name = "Nova Scotia")]
        NovaScotia,
        Ontario,
        [Display(Name = "Prince Edward Island")]
        PrinceEdwardIsland,
        Quebec,
        Saskatchewan
    }
}
