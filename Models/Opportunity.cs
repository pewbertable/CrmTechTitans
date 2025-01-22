using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Opportunity
    {
        public int ID { get; set; }


        [StringLength(200, ErrorMessage = "Opportunity title can't be longer than 200 characters")]
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Priority { get; set; }
    }
}
