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
                ApprovalStatus = user.ApprovalStatus
            };

            return View(viewModel);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                
                // Update approval status if changed
                if (user.ApprovalStatus != model.ApprovalStatus)
                {
                    user.ApprovalStatus = model.ApprovalStatus;
                    user.StatusUpdatedDate = DateTime.Now;
                    
                    // If user was rejected, save the reason
                    if (model.ApprovalStatus == UserApprovalStatus.Rejected && !string.IsNullOrEmpty(model.RejectionReason))
                    {
                        user.RejectionReason = model.RejectionReason;
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Handle role change
                    var userRoles = await _userManager.GetRolesAsync(user);
                    
                    // Remove existing roles
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    
                    // Add new role if selected
                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }

                    // Update password if provided
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
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

            // Don't allow admin to delete themselves
            if (User.Identity.Name == user.UserName)
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