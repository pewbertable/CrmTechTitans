namespace CrmTechTitans.ViewModels
{
    public class DashboardVM
    {
        public MemberCountVM? MemberCountSummary { get; set; }
        public MembershipTypeCountVM? MembershipTypeSummary { get; set; }
        public OpportunityCountVM? OpportunityCountSummary { get; set; }
        public List<MemberCountOverTimeVM>? MemberCountOverTime { get; set; } // New property
    }

    public class MemberCountOverTimeVM
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
