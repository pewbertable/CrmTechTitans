using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data.Utilities
{
    public static class UserStatusUpdater
    {
        /// <summary>
        /// Updates all existing users to have Approved status
        /// </summary>
        public static async Task UpdateExistingUsersToApproved(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            try
            {
                // Get all existing users
                var users = await userManager.Users.ToListAsync();
                int count = 0;
                
                foreach (var user in users)
                {
                    // Only change users who have default Pending status
                    if (user.ApprovalStatus == UserApprovalStatus.Pending)
                    {
                        user.ApprovalStatus = UserApprovalStatus.Approved;
                        user.StatusUpdatedDate = DateTime.Now;
                        
                        var result = await userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            count++;
                        }
                        else
                        {
                            logger.LogError($"Failed to update user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }
                
                logger.LogInformation($"Successfully updated {count} existing users to Approved status");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating existing users to Approved status");
            }
        }
    }
} 