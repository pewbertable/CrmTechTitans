namespace CrmTechTitans.ViewModels
{
    public class MemberCountVM
    {
        public string Membership { get; set; }
        public int TotalMembers { get; set; }
        public int GoodStanding { get; set; }
        public int OutStanding { get; set; }
        public int Cancelled { get; set; }
    }
}
