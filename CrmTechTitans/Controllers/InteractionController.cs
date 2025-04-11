using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;
using CrmTechTitans.Services;
using CrmTechTitans.ViewModels;
using System;
using System.Collections.Generic;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class InteractionController : Controller
    {
        private readonly CrmContext _context;
        private readonly ExcelExportService _excelExportService;

        public InteractionController(CrmContext context, ExcelExportService excelExportService)
        {
            _context = context;
            _excelExportService = excelExportService;
        }

        // GET: Interaction
        public async Task<IActionResult> Index()
        {
            var interactions = await _context.Interactions
                .Include(i => i.Contact)
                .Include(i => i.InteractionMembers)
                    .ThenInclude(im => im.Member)
                .ToListAsync();
            return View(interactions);
        }

        // GET: Interaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interaction = await _context.Interactions
                .Include(i => i.Contact)
                .Include(i => i.InteractionMembers)
                    .ThenInclude(im => im.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (interaction == null)
            {
                return NotFound();
            }

            return View(interaction);
        }

        // GET: Interaction/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Contacts = await _context.Contacts
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
            return View();
        }

        // POST: Interaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create([Bind("Id,InteractionDetails,Date,ContactId")] Interaction interaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(interaction);
                await _context.SaveChangesAsync();
                TempData["success"] = "Interaction created successfully!";
                TempData["message"] = "Interaction created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Contacts = await _context.Contacts.ToListAsync();
            return View(interaction);
        }

        // GET: Interaction/Edit/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interaction = await _context.Interactions.FindAsync(id);
            if (interaction == null)
            {
                return NotFound();
            }
            ViewBag.Contacts = await _context.Contacts
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
            return View(interaction);
        }

        // POST: Interaction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InteractionDetails,Date,ContactId")] Interaction interaction)
        {
            if (id != interaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(interaction);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Interaction updated successfully!";
                    TempData["message"] = "Interaction updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["error"] = "Failed to edit interaction. Please try again.";
                    if (!InteractionExists(interaction.Id))
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
            ViewBag.Contacts = await _context.Contacts.ToListAsync();
            return View(interaction);
        }

        // GET: Interaction/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interaction = await _context.Interactions
                .Include(i => i.Contact)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (interaction == null)
            {
                return NotFound();
            }

            return View(interaction);
        }

        // POST: Interaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var interaction = await _context.Interactions.FindAsync(id);
            if (interaction != null)
            {
                _context.Interactions.Remove(interaction);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Interaction deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult DownloadInteractions([FromForm] InteractionExportOptions options)
        {
            if (options.SelectedFields == null || !options.SelectedFields.Any())
            {
                TempData["error"] = "Please select at least one field to export.";
                return RedirectToAction(nameof(Index));
            }

            IQueryable<Interaction> interactionsQuery = _context.Interactions
                .Include(i => i.Contact)
                .Include(i => i.InteractionMembers).ThenInclude(im => im.Member)
                .AsQueryable();

            if (!options.DownloadAll)
            {
                if (options.SelectedInteractionIds != null && options.SelectedInteractionIds.Any())
                {
                    interactionsQuery = interactionsQuery.Where(i => options.SelectedInteractionIds.Contains(i.Id));
                }
                else
                {
                    TempData["error"] = "No interactions selected for filtered export.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var interactions = interactionsQuery.ToList();

            if (!interactions.Any())
            {
                TempData["warning"] = "No interactions found matching the selected criteria for export.";
                return RedirectToAction(nameof(Index));
            }

            var exportData = new List<Dictionary<string, object>>();
            foreach (var interaction in interactions)
            {
                var interactionData = new Dictionary<string, object>();
                foreach (var field in options.SelectedFields)
                {
                    object value = null;
                    switch (field)
                    {
                        case "InteractionDetails": value = interaction.InteractionDetails; break;
                        case "Date": value = interaction.Date; break;
                        case "ContactName":
                            value = interaction.Contact != null ? $"{interaction.Contact.FirstName} {interaction.Contact.LastName}" : "N/A";
                            break;
                        case "ContactEmail":
                            value = interaction.Contact?.Email ?? "N/A";
                            break;
                        case "AssociatedMembers":
                            value = interaction.InteractionMembers != null && interaction.InteractionMembers.Any()
                                ? string.Join(", ", interaction.InteractionMembers.Select(im => im.Member?.MemberName ?? "N/A"))
                                : "N/A";
                            break;
                        default:
                            break;
                    }
                    interactionData[field] = value;
                }
                exportData.Add(interactionData);
            }

            try
            {
                byte[] fileBytes = _excelExportService.GenerateExcelPackage(
                    exportData,
                    options.SelectedFields,
                    sheetName: "Interactions",
                    reportTitle: "Interaction Report"
                );

                string filename = $"Interactions_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(fileBytes, mimeType, filename);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while generating the Excel file.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool InteractionExists(int id)
        {
            return _context.Interactions.Any(e => e.Id == id);
        }
    }
}
