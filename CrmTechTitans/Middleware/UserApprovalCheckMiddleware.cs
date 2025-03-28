using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CrmTechTitans.Middleware
{
    public class UserApprovalCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public UserApprovalCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            // Check if user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Skip check for these paths
                if (ShouldSkipCheck(context.Request.Path))
                {
                    await _next(context);
                    return;
                }

                // Get user
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Check if 2FA setup is required but not completed
                    if (!user.RegistrationComplete) 
                    {
                        // Redirect to the EnableAuthenticator page
                        context.Response.Redirect("/Identity/Account/Manage/EnableAuthenticator");
                        return;
                    }
                
                    // Check approval status
                    if (user.ApprovalStatus != UserApprovalStatus.Approved)
                    {
                        // Redirect to approval waiting page
                        context.Response.Redirect("/Identity/Account/ApprovalWaiting");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private bool ShouldSkipCheck(string path)
        {
            var pathsToSkip = new[]
            {
                "/Identity/Account/Logout",
                "/Identity/Account/LoggedOut",
                "/Identity/Account/AccessDenied",
                "/Identity/Account/ApprovalWaiting",
                "/Identity/Account/Manage/EnableAuthenticator",
                "/Identity/Account/Manage/ShowRecoveryCodes"
            };

            return pathsToSkip.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class UserApprovalCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserApprovalCheck(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserApprovalCheckMiddleware>();
        }
    }
} 