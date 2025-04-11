using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Services;
using CrmTechTitans.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class OpportunityController : Controller
    {
        private readonly CrmContext _context;
        private readonly ExcelExportService _excelExportService;

        public OpportunityController(CrmContext context, ExcelExportService excelExportService)
        {
            _context = context;
            _excelExportService = excelExportService;
        }

        // GET: Opportunity
        public async Task<IActionResult> Index(Status? statusFilter, PriorityType? priorityFilter)
        {
            var query = _context.Opportunities.Include(c => c.MemberOpportunities)
                .ThenInclude(mc => mc.Member)
                .AsQueryable();

            if (statusFilter.HasValue)
            {
                query = query.Where(o => o.Status == statusFilter.Value);
            }

            if (priorityFilter.HasValue)
            {
                query = query.Where(o => o.Priority == priorityFilter.Value);
            }

            var opportunities = await query.ToListAsync();

            // Pass filters to ViewBag to retain selection in the UI
            ViewBag.StatusFilter = statusFilter;
            ViewBag.PriorityFilter = priorityFilter;

            return View(opportunities);
        }


        // GET: Opportunity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // GET: Opportunity/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Opportunity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create([Bind("ID,Title,Status,Description,Priority")] Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(opportunity);
                await _context.SaveChangesAsync();
                TempData["success"] = "Opportunity created successfully!";
                TempData["message"] = "Opportunity created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(opportunity);
        }

        // GET: Opportunity/Edit/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null)
            {
                return NotFound();
            }
            return View(opportunity);
        }

        // POST: Opportunity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Status,Description,Priority")] Opportunity opportunity)
        {
            if (id != opportunity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Opportunity updated successfully!";
                    TempData["message"] = "Opportunity updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["error"] = "Failed to edit opportunity. Please try again.";

                    if (!OpportunityExists(opportunity.ID))
                    {
                        
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(opportunity);
        }

        // GET: Opportunity/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Opportunity deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult DownloadOpportunities([FromForm] OpportunityExportOptions options)
        {
            if (options.SelectedFields == null || !options.SelectedFields.Any())
            {
                TempData["error"] = "Please select at least one field to export.";
                return RedirectToAction(nameof(Index));
            }

            IQueryable<Opportunity> opportunitiesQuery = _context.Opportunities
                .Include(o => o.MemberOpportunities).ThenInclude(mo => mo.Member)
                .AsQueryable();

            if (!options.DownloadAll)
            {
                if (options.SelectedOpportunityIds != null && options.SelectedOpportunityIds.Any())
                {
                    opportunitiesQuery = opportunitiesQuery.Where(o => options.SelectedOpportunityIds.Contains(o.ID));
                }
                else
                {
                    TempData["error"] = "No opportunities selected for filtered export.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var opportunities = opportunitiesQuery.ToList();

             if (!opportunities.Any())
            {
                TempData["warning"] = "No opportunities found matching the selected criteria for export.";
                return RedirectToAction(nameof(Index));
            }

            var exportData = new List<Dictionary<string, object>>();
            foreach (var opportunity in opportunities)
            {
                var opportunityData = new Dictionary<string, object>();
                foreach (var field in options.SelectedFields)
                {
                    object value = null;
                    switch (field)
                    {
                        case "Title": value = opportunity.Title; break;
                        case "Status": value = opportunity.Status.ToString(); break;
                        case "Description": value = opportunity.Description; break;
                        case "Priority": value = opportunity.Priority.ToString(); break;
                        case "AssociatedMembers":
                             value = opportunity.MemberOpportunities != null && opportunity.MemberOpportunities.Any()
                                ? string.Join(", ", opportunity.MemberOpportunities
                                        .Select(mo => mo.Member?.MemberName)
                                        .Where(name => !string.IsNullOrEmpty(name))
                                        .DefaultIfEmpty("N/A")
                                    )
                                : "N/A";
                             break;
                        default:
                            break;
                    }
                    opportunityData[field] = value;
                }
                exportData.Add(opportunityData);
            }

            try
            {
                byte[] fileBytes = _excelExportService.GenerateExcelPackage(
                    exportData,
                    options.SelectedFields,
                    sheetName: "Opportunities",
                    reportTitle: "Opportunity Report"
                );

                string filename = $"Opportunities_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(fileBytes, mimeType, filename);
            }
            catch (TimeZoneNotFoundException tzEx)
            {
                TempData["error"] = $"Error generating report: Timezone not found. {tzEx.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while generating the Excel file.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.ID == id);
        }
    }
}
