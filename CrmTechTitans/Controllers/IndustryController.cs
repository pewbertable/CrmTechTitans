using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Authorization;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Services;
using CrmTechTitans.ViewModels;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class IndustryController : Controller
    {
        private readonly CrmContext _context;
        private readonly ExcelExportService _excelExportService;

        public IndustryController(CrmContext context, ExcelExportService excelExportService)
        {
            _context = context;
            _excelExportService = excelExportService;
        }

        // GET: Industry
        public async Task<IActionResult> Index(string? SearchString, string? NAICSCode)
        {
            // Count the number of filters applied
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            // Start querying Industries
            var industries = _context.Industries
                .Include(i => i.IndustryMembers)
                .AsNoTracking();

            // Filter by Industry Name
            if (!String.IsNullOrEmpty(SearchString))
            {
                industries = industries.Where(i => i.Name.ToLower().Contains(SearchString.ToLower()));
                numberFilters++;
            }

            // Fix: Convert NAICSCode string to int before filtering
            if (!string.IsNullOrEmpty(NAICSCode) && int.TryParse(NAICSCode, out int naicsCodeInt))
            {
                industries = industries.Where(i => i.NAICS == naicsCodeInt);
                numberFilters++;
            }

            // Provide feedback about applied filters
            if (numberFilters != 0)
            {
                ViewData["Filtering"] = "btn-danger";
                ViewData["numberFilters"] = "(" + numberFilters.ToString() + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                ViewData["ShowFilter"] = "show";
            }

            // Populate NAICS Code dropdown
            ViewBag.NAICSCodeList = new SelectList(await _context.Industries
                .Select(i => i.NAICS.ToString())
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync());

            // Execute the query
            var industryList = await industries.ToListAsync();

            return View(industryList);
        }

        // GET: Industry/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var industry = await _context.Industries
                 .Include(i => i.IndustryMembers) // Include IndustryMembers
            .ThenInclude(im => im.Member) // Include Member entity
                .FirstOrDefaultAsync(m => m.ID == id);
            if (industry == null)
            {
                return NotFound();
            }

            return View(industry);
        }

        // GET: Industry/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Industry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create([Bind("ID,Name,NAICS")] Industry industry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(industry);
                await _context.SaveChangesAsync();
                TempData["success"] = "Industry created successfully!";
                TempData["message"] = "Industry created successfully!";

                return RedirectToAction(nameof(Index));
            }
            return View(industry);
        }

        // GET: Industry/Edit/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var industry = await _context.Industries.FindAsync(id);
            if (industry == null)
            {
                return NotFound();
            }
            return View(industry);
        }

        // POST: Industry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,NAICS")] Industry industry)
        {
            if (id != industry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(industry);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Industry updated successfully!";
                    TempData["message"] = "Industry updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["error"] = "Failed to edit industry. Please try again.";
                    if (!IndustryExists(industry.ID))
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
            return View(industry);
        }

        // GET: Industry/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var industry = await _context.Industries
                .FirstOrDefaultAsync(m => m.ID == id);
            if (industry == null)
            {
                return NotFound();
            }

            return View(industry);
        }

        // POST: Industry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var industry = await _context.Industries.FindAsync(id);
            if (industry != null)
            {
                _context.Industries.Remove(industry);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Industry deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult DownloadIndustries([FromForm] IndustryExportOptions options)
        {
            if (options.SelectedFields == null || !options.SelectedFields.Any())
            {
                TempData["error"] = "Please select at least one field to export.";
                return RedirectToAction(nameof(Index));
            }

            IQueryable<Industry> industriesQuery = _context.Industries
                .Include(i => i.IndustryMembers).ThenInclude(im => im.Member)
                .AsQueryable();

            if (!options.DownloadAll)
            {
                if (options.SelectedIndustryIds != null && options.SelectedIndustryIds.Any())
                {
                    industriesQuery = industriesQuery.Where(i => options.SelectedIndustryIds.Contains(i.ID));
                }
                else
                {
                    TempData["error"] = "No industries selected for filtered export.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var industries = industriesQuery.ToList();

            if (!industries.Any())
            {
                TempData["warning"] = "No industries found matching the selected criteria for export.";
                return RedirectToAction(nameof(Index));
            }

            var exportData = new List<Dictionary<string, object>>();
            foreach (var industry in industries)
            {
                var industryData = new Dictionary<string, object>();
                foreach (var field in options.SelectedFields)
                {
                    object value = null;
                    switch (field)
                    {
                        case "Name": value = industry.Name; break;
                        case "NAICS": value = industry.NAICS; break;
                        case "MemberCount":
                            value = industry.IndustryMembers?.Count() ?? 0;
                            break;
                        case "AssociatedMembers":
                             value = industry.IndustryMembers != null && industry.IndustryMembers.Any()
                                ? string.Join(", ", industry.IndustryMembers.Select(im => im.Member?.MemberName ?? "N/A"))
                                : "N/A";
                             break;
                        default:
                            break;
                    }
                    industryData[field] = value;
                }
                exportData.Add(industryData);
            }

            try
            {
                byte[] fileBytes = _excelExportService.GenerateExcelPackage(
                    exportData,
                    options.SelectedFields,
                    sheetName: "Industries",
                    reportTitle: "Industry Report"
                );

                string filename = $"Industries_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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

        private bool IndustryExists(int id)
        {
            return _context.Industries.Any(e => e.ID == id);
        }
    }
}
