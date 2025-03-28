using CrmTechTitans.Models.Enumerations;

namespace CrmTechTitans.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public UserApprovalStatus ApprovalStatus { get; set; }
        public string StatusLabel
        {
            get
            {
                return ApprovalStatus switch
                {
                    UserApprovalStatus.Approved => "Approved",
                    UserApprovalStatus.Pending => "Pending",
                    UserApprovalStatus.Rejected => "Rejected",
                    _ => "Unknown"
                };
            }
        }
        public string StatusClass
        {
            get
            {
                return ApprovalStatus switch
                {
                    UserApprovalStatus.Approved => "success",
                    UserApprovalStatus.Pending => "warning",
                    UserApprovalStatus.Rejected => "danger",
                    _ => "secondary"
                };
            }
        }
        public string RejectionReason { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool RegistrationComplete { get; set; }
    }
} 