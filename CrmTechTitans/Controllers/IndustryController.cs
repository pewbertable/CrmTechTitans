﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace CrmTechTitans.Controllers
{
    public class IndustryController : Controller
    {
        private readonly CrmContext _context;

        public IndustryController(CrmContext context)
        {
            _context = context;
        }

        // GET: Industry
        public async Task<IActionResult> Index(string? SearchString, string? NAICSCode,
     string? actionButton, string sortDirection = "asc", string sortField = "Name")
        {
            // List of sort options
            string[] sortOptions = new[] { "Name", "NAICS" };

            // Count the number of filters applied
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            // Start querying Industries
            var industries = _context.Industries.AsNoTracking();

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

            // Sorting logic
            if (!String.IsNullOrEmpty(actionButton))
            {
                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;
                }
            }

            // Apply Sorting
            if (sortField == "NAICS")
            {
                industries = sortDirection == "asc"
                    ? industries.OrderBy(i => i.NAICS).ThenBy(i => i.Name)
                    : industries.OrderByDescending(i => i.NAICS).ThenByDescending(i => i.Name);
            }
            else // Default: Sorting by Industry Name
            {
                industries = sortDirection == "asc"
                    ? industries.OrderBy(i => i.Name).ThenBy(i => i.NAICS)
                    : industries.OrderByDescending(i => i.Name).ThenByDescending(i => i.NAICS);
            }

            // Set sort for next request
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Industry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,NAICS")] Industry industry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(industry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(industry);
        }

        // GET: Industry/Edit/5
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
                    TempData["message"] = "Industry edited successfully";

                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["errMessage"] = "An error occured. Failed to edit the industry.";
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var industry = await _context.Industries.FindAsync(id);
            if (industry != null)
            {
                _context.Industries.Remove(industry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IndustryExists(int id)
        {
            return _context.Industries.Any(e => e.ID == id);
        }
    }
}
