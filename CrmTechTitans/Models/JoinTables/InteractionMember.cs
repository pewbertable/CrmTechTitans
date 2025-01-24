namespace CrmTechTitans.Models.JoinTables
{
    public class InteractionMember
    {
        public int InteractionMemberID { get; set; }
        public int MemberID { get; set; }
        public Member? Member { get; set; }
        public int InteractionID { get; set; }
        public Interaction? Interaction { get; set; }
        
        

    }
}
