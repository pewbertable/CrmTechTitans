using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace CrmTechTitans.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Current approval status of the user
        /// </summary>
        public UserApprovalStatus ApprovalStatus { get; set; } = UserApprovalStatus.Pending;

        /// <summary>
        /// Reason for rejection if the user was rejected
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Date when the user's status was last updated
        /// </summary>
        public DateTime? StatusUpdatedDate { get; set; }
        
        /// <summary>
        /// Whether the user has completed initial registration process including 2FA setup
        /// </summary>
        public bool RegistrationComplete { get; set; } = false;
    }
} 