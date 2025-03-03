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
            var memberSummaryList = await _context.Members
                .GroupBy(m => m.MembershipStatus)
                .Select(grp => new MemberCountVM
                {
                    Membership = grp.Key.ToString(),
                    TotalMembers = grp.Count(),
                    GoodStanding = grp.Count(m => m.MembershipStatus == MembershipStatus.GoodStanding),
                    Cancelled = grp.Count(m => m.MembershipStatus == MembershipStatus.Cancelled),
                    OutStanding = grp.Count(m => m.MembershipStatus == MembershipStatus.OutStanding)
                }).ToListAsync();

            // Aggregate results manually to ensure all data is considered
            var memberSummary = new MemberCountVM
            {
                Membership = "Summary",
                TotalMembers = memberSummaryList.Sum(x => x.TotalMembers),
                GoodStanding = memberSummaryList.Sum(x => x.GoodStanding),
                Cancelled = memberSummaryList.Sum(x => x.Cancelled),
                OutStanding = memberSummaryList.Sum(x => x.OutStanding)
            };

            var membershipSummaryList = await _context.MemberMembershipTypes
                .GroupBy(mmt => mmt.MembershipType.Name)
                .Select(grp => new
                {
                    Name = grp.Key.ToLower(),
                    Count = grp.Count()
                }).ToListAsync();

            var membershipSummary = new MembershipTypeCountVM
            {
                AssociateCount = membershipSummaryList.Where(x => x.Name == "associate").Sum(x => x.Count),
                ChamberAssociateCount = membershipSummaryList.Where(x => x.Name == "chamberassociate").Sum(x => x.Count),
                NonLocalIndustrialCount = membershipSummaryList.Where(x => x.Name == "nonlocalindustrial").Sum(x => x.Count),
                GovernmentAssociationCount = membershipSummaryList.Where(x => x.Name == "governmenteducationassociate").Sum(x => x.Count),
                LocalCount = membershipSummaryList.Where(x => x.Name == "localindustrial").Sum(x => x.Count),
                OtherCount = membershipSummaryList.Where(x => x.Name == "other").Sum(x => x.Count)
            };

            var opportunitySummaryList = await _context.Opportunities
                .GroupBy(o => o.Status)
                .Select(grp => new OpportunityCountVM
                {
                    QualificationCount = grp.Count(o => o.Status == Status.Qualification),
                    NegotiationCount = grp.Count(o => o.Status == Status.Negotiating),
                    ClosedNewMembersCount = grp.Count(o => o.Status == Status.ClosedNewMember),
                    ClosedNotInterestedCount = grp.Count(o => o.Status == Status.ClosedNotInterested)
                }).ToListAsync();

            // Aggregate results manually to ensure all data is considered
            var opportunitySummary = new OpportunityCountVM
            {
                QualificationCount = opportunitySummaryList.Sum(x => x.QualificationCount),
                NegotiationCount = opportunitySummaryList.Sum(x => x.NegotiationCount),
                ClosedNewMembersCount = opportunitySummaryList.Sum(x => x.ClosedNewMembersCount),
                ClosedNotInterestedCount = opportunitySummaryList.Sum(x => x.ClosedNotInterestedCount)
            };

            // Ensure dates are serialized to ISO strings
            var memberCountOverTime = await _context.Members
                .GroupBy(m => m.MemberSince.Date)
                .Select(grp => new MemberCountOverTimeVM
                {
                    Date = grp.Key, // DateTime object
                    Count = grp.Count()
                })
                .OrderBy(m => m.Date)
                .ToListAsync();

            // Combine all summaries into a single view model
            var dashboardViewModel = new DashboardVM
            {
                MemberCountSummary = memberSummary,
                MembershipTypeSummary = membershipSummary,
                OpportunityCountSummary = opportunitySummary,
                MemberCountOverTime = memberCountOverTime // New data
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
