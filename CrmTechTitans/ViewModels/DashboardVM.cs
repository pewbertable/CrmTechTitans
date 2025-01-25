namespace CrmTechTitans.ViewModels
{
    public class DashboardVM
    {
        public MemberCountVM? MemberCountSummary { get; set; }
        public MembershipTypeCountVM? MembershipTypeSummary { get; set; }
        public OpportunityCountVM? OpportunityCountSummary { get; set; }
    }
}
