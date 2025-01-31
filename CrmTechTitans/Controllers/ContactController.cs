using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;

namespace CrmTechTitans.Controllers
{
    public class ContactController : Controller
    {
        private readonly CrmContext _context;

        public ContactController(CrmContext context)
        {
            _context = context;
        }

        // GET: Contact
        public async Task<IActionResult> Index(string? SearchString,
     string? actionButton, string sortDirection = "asc", string sortField = "FirstName")
        {
            // List of sort options for sorting by FirstName or LastName
            string[] sortOptions = new[] { "FirstName", "LastName" };

            // Count the number of filters applied 
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

          var contacts = _context.Contacts
            .Include(c => c.MemberContacts)
            .ThenInclude(mc => mc.Member)
             .AsNoTracking();

            //Add Filtering
            if (!String.IsNullOrEmpty(SearchString))
            {
                contacts = contacts.Where(c => c.FirstName.ToLower().Contains(SearchString.ToLower()) || c.LastName.ToLower().Contains(SearchString.ToLower()));
                numberFilters++;
            }

            // Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                
                ViewData["Filtering"] = "btn-danger";
                ViewData["numberFilters"] = "(" + numberFilters.ToString() + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                ViewData["ShowFilter"] = "show";
            }

            //check if a sort change has been requested
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

            // Apply sorting based on the selected field and direction
            if (sortField == "LastName")
            {
                contacts = sortDirection == "asc"
                    ? contacts.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                    : contacts.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName);
            }
            else // Sorting by FirstName
            {
                contacts = sortDirection == "asc"
                    ? contacts.OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
                    : contacts.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.LastName);
            }

            // Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            // Execute the query and get the result
            var contactsList = await contacts.ToListAsync();

            return View(contactsList);
        }

        // GET: Contact/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.MemberContacts) // Include MemberContacts
            .ThenInclude(mc => mc.Member) // Include Member entity
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Phone,Linkedin")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,Phone,Linkedin")] Contact contact)
        {
            if (id != contact.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ID))
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
            return View(contact);
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }
    }
}
