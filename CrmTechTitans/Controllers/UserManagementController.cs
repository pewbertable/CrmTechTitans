using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    [Authorize(Roles = UserRoles.Administrator)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList(),
                    ApprovalStatus = user.ApprovalStatus,
                    RegistrationComplete = user.RegistrationComplete
                });
            }

            return View(userViewModels);
        }

        // GET: UserManagement/Create
        public IActionResult Create()
        {
            var viewModel = new CreateUserViewModel
            {
                AvailableRoles = UserRoles.AllRoles.ToList()
            };
            return View(viewModel);
        }

        // POST: UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    ApprovalStatus = UserApprovalStatus.Approved, // Admin-created users are auto-approved
                    StatusUpdatedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AvailableRoles = UserRoles.AllRoles.ToList();
            return View(model);
        }

        // GET: UserManagement/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                SelectedRole = userRoles.FirstOrDefault(),
                AvailableRoles = UserRoles.AllRoles.ToList(),
                ApprovalStatus = user.ApprovalStatus,
                NewPassword = string.Empty,
                ConfirmNewPassword = string.Empty
            };

            return View(viewModel);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            _logger.LogInformation("Edit action started for user ID: {UserId}", id);
            _logger.LogInformation("Model state is valid: {IsValid}", ModelState.IsValid);
            _logger.LogInformation("Model data - Email: {Email}, Username: {Username}, SelectedRole: {SelectedRole}, ApprovalStatus: {ApprovalStatus}",
                model.Email, model.UserName, model.SelectedRole, model.ApprovalStatus);

            if (id != model.Id)
            {
                _logger.LogError("ID mismatch - Route ID: {RouteId}, Model ID: {ModelId}", id, model.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogError("User not found with ID: {UserId}", id);
                return NotFound();
            }

            _logger.LogInformation("Found user - Email: {Email}, Username: {Username}", user.Email, user.UserName);

            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogError("Current user not found");
                return NotFound();
            }

            // Check if current user is trying to change their own role
            if (currentUser.Id == user.Id)
            {
                _logger.LogWarning("User {UserId} attempted to change their own role", currentUser.Id);
                ModelState.AddModelError("", "You cannot change your own role.");
                return View(model);
            }

            // Get current user's roles
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            _logger.LogInformation("Current user roles: {Roles}", string.Join(", ", currentUserRoles));

            // Get target user's current roles
            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Target user current roles: {Roles}", string.Join(", ", userRoles));

            // Check if target user is an admin
            bool isTargetUserAdmin = userRoles.Contains("Admin");

            // If target user is an admin, only allow other admins to change their role
            if (isTargetUserAdmin && !currentUserRoles.Contains("Admin"))
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to change admin user {TargetUserId}'s role", 
                    currentUser.Id, user.Id);
                ModelState.AddModelError("", "Only administrators can modify other administrators' roles.");
                return View(model);
            }

            // Update role if changed
            if (model.SelectedRole != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(model.SelectedRole))
                {
                    _logger.LogInformation("Updating role from {CurrentRoles} to {NewRole}", 
                        string.Join(", ", currentRoles), model.SelectedRole);
                    var roleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed to remove current roles: {Errors}", 
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }

                    roleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed to add new role: {Errors}", 
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
            }

            // Then handle email update
            if (user.Email != model.Email)
            {
                _logger.LogInformation($"Updating email from '{user.Email}' to '{model.Email}'");
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded)
                {
                    _logger.LogError($"Failed to update email: {string.Join(", ", emailResult.Errors.Select(e => e.Description))}");
                    foreach (var error in emailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    model.AvailableRoles = UserRoles.AllRoles.ToList();
                    return View(model);
                }
            }

            // Then handle username update
            if (user.UserName != model.UserName)
            {
                _logger.LogInformation($"Updating username from '{user.UserName}' to '{model.UserName}'");
                var usernameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!usernameResult.Succeeded)
                {
                    _logger.LogError($"Failed to update username: {string.Join(", ", usernameResult.Errors.Select(e => e.Description))}");
                    foreach (var error in usernameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    model.AvailableRoles = UserRoles.AllRoles.ToList();
                    return View(model);
                }
            }

            // Handle approval status update
            if (user.ApprovalStatus != model.ApprovalStatus)
            {
                _logger.LogInformation($"Updating approval status from '{user.ApprovalStatus}' to '{model.ApprovalStatus}'");
                user.ApprovalStatus = model.ApprovalStatus;
                user.StatusUpdatedDate = DateTime.Now;
                
                if (model.ApprovalStatus == UserApprovalStatus.Rejected && !string.IsNullOrEmpty(model.RejectionReason))
                {
                    user.RejectionReason = model.RejectionReason;
                }
            }

            // Handle password update if provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                _logger.LogInformation("Updating password");
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    _logger.LogError($"Failed to update password: {string.Join(", ", passwordResult.Errors.Select(e => e.Description))}");
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    model.AvailableRoles = UserRoles.AllRoles.ToList();
                    return View(model);
                }
            }

            // Finally, update the user
            _logger.LogInformation("Saving final user update");
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User updated successfully");
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogError($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.AvailableRoles = UserRoles.AllRoles.ToList();
            return View(model);
        }

        // GET: UserManagement/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                ApprovalStatus = user.ApprovalStatus,
                RegistrationComplete = user.RegistrationComplete
            };

            return View(viewModel);
        }

        // POST: UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow any user to delete their own account
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                ModelState.AddModelError(string.Empty, "You cannot delete your own account.");
                var roles = await _userManager.GetRolesAsync(user);
                return View(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList(),
                    ApprovalStatus = user.ApprovalStatus,
                    RegistrationComplete = user.RegistrationComplete
                });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            return View(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = userRoles.ToList(),
                ApprovalStatus = user.ApprovalStatus,
                RegistrationComplete = user.RegistrationComplete
            });
        }
        
        // GET: UserManagement/PendingApprovals
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingUsers = await _userManager.Users
                .Where(u => u.ApprovalStatus == UserApprovalStatus.Pending)
                .ToListAsync();
            
            var userViewModels = new List<UserViewModel>();

            foreach (var user in pendingUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList(),
                    ApprovalStatus = user.ApprovalStatus,
                    RegistrationDate = user.StatusUpdatedDate,
                    RegistrationComplete = user.RegistrationComplete
                });
            }

            return View(userViewModels);
        }
        
        // POST: UserManagement/ApproveUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveUser(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if user has completed 2FA setup
            if (!user.RegistrationComplete)
            {
                TempData["ErrorMessage"] = $"User {user.Email} has not completed 2FA setup and cannot be approved.";
                return RedirectToAction(nameof(PendingApprovals));
            }

            // Update user approval status
            user.ApprovalStatus = UserApprovalStatus.Approved;
            user.StatusUpdatedDate = DateTime.Now;
            
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                // Assign the role if provided
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    // Default to ReadOnly if no role specified
                    await _userManager.AddToRoleAsync(user, UserRoles.ReadOnly);
                }
                
                // Send approval notification email
                try
                {
                    var callbackUrl = Url.Action("Login", "Account", new { area = "Identity" }, protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Your Account Has Been Approved",
                        $"<p>Your account registration for CRM Tech Titans has been approved by an administrator.</p>" +
                        $"<p>You can now <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>log in</a> to access the system.</p>" +
                        $"<p>Thank you for your patience.</p>");
                    
                    _logger.LogInformation($"Approval notification email sent to {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending approval notification to {user.Email}");
                    // Continue even if email fails
                }
                
                TempData["SuccessMessage"] = $"User {user.Email} has been approved.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Failed to approve user.";
            }

            return RedirectToAction(nameof(PendingApprovals));
        }
        
        // POST: UserManagement/RejectUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectUser(string id, string reason)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user approval status
            user.ApprovalStatus = UserApprovalStatus.Rejected;
            user.RejectionReason = reason;
            user.StatusUpdatedDate = DateTime.Now;
            
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.Email} has been rejected.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Failed to reject user.";
            }

            return RedirectToAction(nameof(PendingApprovals));
        }

        // GET: UserManagement/CreateTestAccount
        public IActionResult CreateTestAccount()
        {
            var viewModel = new CreateUserViewModel
            {
                AvailableRoles = UserRoles.AllRoles.ToList()
            };
            return View("CreateTestAccount", viewModel);
        }

        // POST: UserManagement/CreateTestAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTestAccount(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Use the IdentityInitializer to create a test account with 2FA disabled
                var result = await Data.IdentityInitializer.CreateTestAccountAsync(
                    _userManager,
                    model.Email,
                    model.Password,
                    model.SelectedRole ?? UserRoles.Administrator
                );

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Test account {model.Email} created successfully without 2FA requirement.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AvailableRoles = UserRoles.AllRoles.ToList();
            return View("CreateTestAccount", model);
        }
    }
} 