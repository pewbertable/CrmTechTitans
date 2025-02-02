using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using CrmTechTitans.Models.ViewModels;
using CrmTechTitans.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    public class MemberController : Controller
    {
        private readonly CrmContext _context;

        public MemberController(CrmContext context)
        {
            _context = context;
        }



        // Other actions...

        // GET: Member
        public async Task<IActionResult> Index(string searchString, string statusFilter, string sortField, string sortDirection)
        {
            // Start with the base query
            var members = _context.Members
                .Include(m => m.IndustryMembers)
                    .ThenInclude(im => im.Industry)
                .AsQueryable();

            // Filter by Member Name
            if (!string.IsNullOrEmpty(searchString))
            {
                members = members.Where(m => m.MemberName.Contains(searchString));
            }

            // Filter by Member Status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var status = Enum.Parse<MembershipStatus>(statusFilter);
                members = members.Where(m => m.MembershipStatus == status);
            }

            // Sorting
            switch (sortField)
            {
                case "MemberName":
                    members = sortDirection == "asc" ? members.OrderBy(m => m.MemberName) : members.OrderByDescending(m => m.MemberName);
                    break;
                case "MembershipStatus":
                    members = sortDirection == "asc" ? members.OrderBy(m => m.MembershipStatus) : members.OrderByDescending(m => m.MembershipStatus);
                    break;
                default:
                    members = members.OrderBy(m => m.MemberName); // Default sorting
                    break;
            }

            // Pass sorting and filtering data to the view
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["SearchString"] = searchString;
            ViewData["statusFilter"] = statusFilter;

            return View(await members.ToListAsync());
        }

        // Other actions...
    }
}
