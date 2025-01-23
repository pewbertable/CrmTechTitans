using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;

namespace CrmTechTitans.Models.JoinTables
{
    // MemberAddress join table
    public class MemberAddress
    {
        public int MemberId { get; set; }
        public required Member Member { get; set; }

        public int AddressId { get; set; }
        public required Address Address { get; set; }

        public AddressType AddressType { get; set; }
    }
}
