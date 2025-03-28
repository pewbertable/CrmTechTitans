using CrmTechTitans.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrmTechTitans.Areas.Identity.Pages.Account
{
    public class ApprovalWaitingModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApprovalWaitingModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
            // If the user is not authenticated, redirect to login
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Identity/Account/Login");
                return;
            }
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/Identity/Account/Login");
        }
    }
} 