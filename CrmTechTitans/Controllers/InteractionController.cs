using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;

namespace CrmTechTitans.Controllers
{
    public class InteractionController : Controller
    {
        private readonly CrmContext _context;

        public InteractionController(CrmContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("Id,InteractionDetails,Date,ContactId")] Interaction interaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(interaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Contacts = await _context.Contacts.ToListAsync();
            return View(interaction);
        }

        // GET: Interaction/Edit/5
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
                    TempData["message"] = "Interaction edited successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["errMessage"] = "An error occurred. Failed to edit the interaction.";
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var interaction = await _context.Interactions.FindAsync(id);
            if (interaction != null)
            {
                _context.Interactions.Remove(interaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InteractionExists(int id)
        {
            return _context.Interactions.Any(e => e.Id == id);
        }
    }
}
