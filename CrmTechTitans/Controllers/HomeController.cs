using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CrmContext _context;

        public HomeController(ILogger<HomeController> logger, CrmContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve data from the database asynchronously
            var memberSummary = await _context.Members
                .GroupBy(m => m.MembershipStatus)
                .Select(grp => new MemberCountVM
                {
                    Membership = grp.Key.ToString(),
                    TotalMembers = grp.Count(),
                    ActiveMembers = grp.Count(m => m.MembershipStatus == MembershipStatus.Active),
                    InactiveMembers = grp.Count(m => m.MembershipStatus == MembershipStatus.Inactive)
                }).FirstOrDefaultAsync();

            var membershipSummary = await _context.Members
                .GroupBy(m => m.MembershipType)
                .Select(grp => new MembershipTypeCountVM
                {
                    AssociateCount = grp.Count(m => m.MembershipType == MembershipType.Associate),
                    ChamberAssociateCount = grp.Count(m => m.MembershipType == MembershipType.ChamberAssociate),
                    NonLocalIndustrialCount = grp.Count(m => m.MembershipType == MembershipType.NonLocalIndustrial),
                    GovernmentAssociationCount = grp.Count(m => m.MembershipType == MembershipType.GovernmentEducationAssociate),
                    LocalCount = grp.Count(m => m.MembershipType == MembershipType.Localindustrial)
                }).FirstOrDefaultAsync();

            var opportunitySummary = await _context.Opportunities
                .GroupBy(o => o.Status)
                .Select(grp => new OpportunityCountVM
                {
                    QualificationCount = grp.Count(o => o.Status == Status.Qualification),
                    NegotiationCount = grp.Count(o => o.Status == Status.Negotiating),
                    ClosedNewMembersCount = grp.Count(o => o.Status == Status.ClosedNewMember),
                    ClosedNotInterestedCount = grp.Count(o => o.Status == Status.ClosedNotInterested)
                }).FirstOrDefaultAsync();

            // Combine all summaries into a single view model
            var dashboardViewModel = new DashboardVM
            {
                MemberCountSummary = memberSummary,
                MembershipTypeSummary = membershipSummary,
                OpportunityCountSummary = opportunitySummary
            };

            return View(dashboardViewModel);
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
