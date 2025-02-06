using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class MemberPhoto
    {
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public byte[]? Content { get; set; }

        [StringLength(255)]
        public string? MimeType { get; set; }

        public int MemberID { get; set; }
        public Member? Member { get; set; }

    }
}
