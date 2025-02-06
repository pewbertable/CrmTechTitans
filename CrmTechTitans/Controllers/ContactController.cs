using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.ViewModels;
using CrmTechTitans.Utilities;

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
           

          var contacts = _context.Contacts
            .Include(c => c.ContactThumbnail)
            .Include(c => c.MemberContacts)
            .ThenInclude(mc => mc.Member)
             .AsNoTracking();

           

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
                .Include(c => c.ContactPhoto)
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
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Phone,Linkedin")] Contact contact, IFormFile contactPicture)
        {
            if (ModelState.IsValid)
            {
                await AddContactPicture(contact, contactPicture);
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

            var contact = await _context.Contacts
                .Include (c => c.ContactPhoto)
                .FirstOrDefaultAsync(c => c.ID == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,Phone,Linkedin")] Contact contact, string? chkRemoveContactImage, IFormFile? contactPicture)
        {
            if (id != contact.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //For the image
                    if (chkRemoveContactImage != null)
                    {
                        
                        contact.ContactThumbnail = _context.ContactThumbnails.Where(c => c.ContactID == contact.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        contact.ContactPhoto = null;
                        contact.ContactThumbnail = null;
                    }
                    else
                    {
                        await AddContactPicture(contact, contactPicture);
                    }
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Contact edited successfully";

                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["errMessage"] = "An error occured. Failed to edit the contact.";
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
                .Include(c => c.ContactPhoto)
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
            var contact = await _context.Contacts
                .Include(c => c.ContactPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
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

        private async Task AddContactPicture(Contact contact, IFormFile thePicture)
        {
            //Get the picture and save it with the Member (2 sizes)
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();//Gives us the Byte[]

                        //Check if we are replacing or creating new
                        if (contact.ContactPhoto != null)
                        {
                            //We already have pictures so just replace the Byte[]
                            contact.ContactPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);

                            //Get the Thumbnail so we can update it.  Remember we didn't include it
                            contact.ContactThumbnail = _context.ContactThumbnails.Where(p => p.ContactID == contact.ID).FirstOrDefault();
                            if (contact.ContactThumbnail != null)
                            {
                                contact.ContactThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70);
                            }
                        }
                        else //No pictures saved so start new
                        {
                            contact.ContactPhoto = new ContactPhoto
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                            contact.ContactThumbnail = new ContactThumbnail
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }
    }
}
