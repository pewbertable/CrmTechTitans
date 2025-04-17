using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using CrmTechTitans.Models;
using CrmTechTitans.Services;
using System.Text;

namespace CrmTechTitans.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _sender;
        private readonly IWebHostEnvironment _environment;
        private readonly EmailSettings _emailSettings;

        public RegisterConfirmationModel(
            UserManager<ApplicationUser> userManager, 
            IEmailSender sender,
            IWebHostEnvironment environment,
            IOptions<EmailSettings> emailSettings)
        {
            _userManager = userManager;
            _sender = sender;
            _environment = environment;
            _emailSettings = emailSettings.Value;
        }

        public string Email { get; set; } = string.Empty;

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string email, string? returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            
            // Display confirmation link based on environment and configuration
            var isDevelopmentMode = _environment.IsDevelopment() || _emailSettings.DevelopmentMode;
            var shouldGenerateLinks = isDevelopmentMode && 
                (_emailSettings.Development?.GenerateLinks != false); // Default to true if not specified
            
            DisplayConfirmAccountLink = shouldGenerateLinks;
            
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
} 