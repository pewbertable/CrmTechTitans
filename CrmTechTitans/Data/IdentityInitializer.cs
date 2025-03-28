using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
{
    public static class IdentityInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            // Create roles if they don't exist
            await CreateRolesAsync(roleManager);
            
            // Create test users for each role
            await CreateTestUsersAsync(userManager);
            
            // Also ensure any existing users are approved (in case they were created before this update)
            await EnsureAllSystemUsersAreApproved(userManager);
        }
        
        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in UserRoles.AllRoles)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        
        private static async Task CreateTestUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Create a test user for Administrator role
            await CreateUserIfNotExistsAsync(userManager, "admin@crmtechtitans.com", "admin", UserRoles.Administrator);
            
            // Create a test user for Editor role
            await CreateUserIfNotExistsAsync(userManager, "editor@crmtechtitans.com", "editor", UserRoles.Editor);
            
            // Create a test user for ReadOnly role
            await CreateUserIfNotExistsAsync(userManager, "readonly@crmtechtitans.com", "readonly", UserRoles.ReadOnly);
        }
        
        private static async Task CreateUserIfNotExistsAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    ApprovalStatus = UserApprovalStatus.Approved, // Test users are automatically approved
                    StatusUpdatedDate = DateTime.Now,
                    TwoFactorEnabled = false, // Disable 2FA for test accounts
                    RegistrationComplete = true // Mark registration as complete to bypass 2FA setup
                };
                
                var result = await userManager.CreateAsync(newUser, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
                else
                {
                    // Log the error
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Error creating user {email}: {errors}");
                }
            }
            else if (user.ApprovalStatus != UserApprovalStatus.Approved)
            {
                // Ensure existing admin/editor users are always approved
                user.ApprovalStatus = UserApprovalStatus.Approved;
                user.StatusUpdatedDate = DateTime.Now;
                
                // Ensure 2FA is disabled for test accounts
                if (!user.TwoFactorEnabled)
                {
                    user.TwoFactorEnabled = false;
                    user.RegistrationComplete = true;
                }
                
                await userManager.UpdateAsync(user);
            }
        }
        
        private static async Task EnsureAllSystemUsersAreApproved(UserManager<ApplicationUser> userManager)
        {
            // Find all users with admin or editor roles
            var adminUsers = await userManager.GetUsersInRoleAsync(UserRoles.Administrator);
            var editorUsers = await userManager.GetUsersInRoleAsync(UserRoles.Editor);
            
            var systemUsers = adminUsers.Union(editorUsers).ToList();
            
            foreach (var user in systemUsers)
            {
                // Make sure they're approved
                if (user.ApprovalStatus != UserApprovalStatus.Approved)
                {
                    user.ApprovalStatus = UserApprovalStatus.Approved;
                    user.StatusUpdatedDate = DateTime.Now;
                    await userManager.UpdateAsync(user);
                    Console.WriteLine($"Updated system user {user.Email} to Approved status");
                }
            }
        }
        
        /// <summary>
        /// Creates a test admin account without 2FA requirement for testing purposes
        /// </summary>
        /// <param name="userManager">User manager service</param>
        /// <param name="email">Email for the test account</param>
        /// <param name="password">Password for the test account</param>
        /// <param name="role">Role for the test account (default: Administrator)</param>
        /// <returns>Result of the account creation</returns>
        public static async Task<IdentityResult> CreateTestAccountAsync(
            UserManager<ApplicationUser> userManager, 
            string email, 
            string password, 
            string role = UserRoles.Administrator)
        {
            // Check if user already exists
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                // Return error if user already exists
                return IdentityResult.Failed(new IdentityError { 
                    Description = $"User with email {email} already exists."
                });
            }
            
            // Create new test user with 2FA disabled
            var newUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                ApprovalStatus = UserApprovalStatus.Approved,
                StatusUpdatedDate = DateTime.Now,
                TwoFactorEnabled = false,
                RegistrationComplete = true
            };
            
            // Create the user
            var result = await userManager.CreateAsync(newUser, password);
            
            if (result.Succeeded)
            {
                // Add user to the specified role
                await userManager.AddToRoleAsync(newUser, role);
            }
            
            return result;
        }
    }
} 