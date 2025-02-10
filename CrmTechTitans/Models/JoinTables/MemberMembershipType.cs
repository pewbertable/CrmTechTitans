namespace CrmTechTitans.Models.JoinTables
{
    public class MemberMembershipType
    {
        public int MemberID { get; set; }
        public Member Member { get; set; }

        public int MembershipTypeID { get; set; }
        public MembershipType MembershipType { get; set; }
    }
}
