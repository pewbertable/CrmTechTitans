using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class ContactPhoto
    {
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public byte[]? Content { get; set; }

        [StringLength(255)]
        public string? MimeType { get; set; }

        public int ContactID { get; set; }
        public Contact? Contact { get; set; }

    }
}
