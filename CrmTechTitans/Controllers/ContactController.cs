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
using Microsoft.AspNetCore.Authorization;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using CrmTechTitans.Services;
using CrmTechTitans.ViewModels;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly CrmContext _context;
        private readonly ExcelExportService _excelExportService;

        public ContactController(CrmContext context, ExcelExportService excelExportService)
        {
            _context = context;
            _excelExportService = excelExportService;
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
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create(int? memberId, string? returnUrl)
        {
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Phone,ContactType")] Contact contact, int? memberId, string? returnUrl, IFormFile? contactPicture)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate contact first
                if (!string.IsNullOrEmpty(contact.FirstName) && !string.IsNullOrEmpty(contact.LastName))
                {
                    var existingContact = await _context.Contacts
                        .FirstOrDefaultAsync(c => 
                            c.FirstName.ToLower() == contact.FirstName.ToLower() && 
                            c.LastName.ToLower() == contact.LastName.ToLower() &&
                            (string.IsNullOrEmpty(c.Email) ? string.IsNullOrEmpty(contact.Email) : 
                             c.Email.ToLower() == (contact.Email == null ? "" : contact.Email.ToLower())));
                    
                    if (existingContact != null)
                    {
                        ModelState.AddModelError("", "A contact with this name and email already exists.");
                        TempData["error"] = "A contact with this name and email already exists.";
                        ViewBag.MemberId = memberId;
                        ViewBag.ReturnUrl = returnUrl;
                        return View(contact);
                    }
                }

                // Process contact picture if any
                await AddContactPicture(contact, contactPicture);
                
                _context.Add(contact);
                await _context.SaveChangesAsync();

                // If memberId is provided, create the relationship with the member
                if (memberId.HasValue)
                {
                    var memberContact = new MemberContact
                    {
                        MemberID = memberId.Value,
                        ContactID = contact.ID,
                        ContactType = contact.ContactType ?? ContactType.General
                    };
                    _context.MemberContacts.Add(memberContact);
                    await _context.SaveChangesAsync();
                }

                TempData["success"] = "Contact created successfully!";
                TempData["message"] = "Contact created successfully!";

                // Return to the specified URL or default to the Contact Index
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;
            return View(contact);
        }

        // GET: Contact/Edit/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ContactPhoto)
                .Include(c => c.ContactThumbnail)
                .FirstOrDefaultAsync(m => m.ID == id);
                
            if (contact == null)
            {
                return NotFound();
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,Phone,ContactType")] Contact contact, string? returnUrl, string? chkRemoveContactImage, IFormFile? contactPicture)
        {
            if (id != contact.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate contact (excluding the current contact)
                if (!string.IsNullOrEmpty(contact.FirstName) && !string.IsNullOrEmpty(contact.LastName))
                {
                    var existingContact = await _context.Contacts
                        .FirstOrDefaultAsync(c => 
                            c.ID != id &&
                            c.FirstName.ToLower() == contact.FirstName.ToLower() && 
                            c.LastName.ToLower() == contact.LastName.ToLower() &&
                            (string.IsNullOrEmpty(c.Email) ? string.IsNullOrEmpty(contact.Email) : 
                             c.Email.ToLower() == (contact.Email == null ? "" : contact.Email.ToLower())));
                    
                    if (existingContact != null)
                    {
                        ModelState.AddModelError("", "A contact with this name and email already exists.");
                        TempData["error"] = "A contact with this name and email already exists.";
                        ViewBag.ReturnUrl = returnUrl;
                        return View(contact);
                    }
                }

                try
                {
                    // Get existing contact with photo
                    var existingContact = await _context.Contacts
                        .Include(c => c.ContactPhoto)
                        .Include(c => c.ContactThumbnail)
                        .FirstOrDefaultAsync(c => c.ID == id);
                        
                    if (existingContact == null)
                    {
                        return NotFound();
                    }

                    // Update contact properties
                    existingContact.FirstName = contact.FirstName;
                    existingContact.LastName = contact.LastName;
                    existingContact.Email = contact.Email;
                    existingContact.Phone = contact.Phone;
                    existingContact.ContactType = contact.ContactType;

                    // Handle image updates
                    if (chkRemoveContactImage != null)
                    {
                        if (existingContact.ContactPhoto != null)
                            _context.ContactPhotos.Remove(existingContact.ContactPhoto);
                        if (existingContact.ContactThumbnail != null)
                            _context.ContactThumbnails.Remove(existingContact.ContactThumbnail);
                        existingContact.ContactPhoto = null;
                        existingContact.ContactThumbnail = null;
                    }
                    else if (contactPicture != null)
                    {
                        await AddContactPicture(existingContact, contactPicture);
                    }

                    _context.Update(existingContact);
                    await _context.SaveChangesAsync();
                    
                    TempData["success"] = "Contact updated successfully!";
                    TempData["message"] = "Contact updated successfully!";

                    // Return to the specified URL or default to the Contact Index
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Index));
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
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View(contact);
        }

        // GET: Contact/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
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
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
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
            TempData["success"] = "Contact deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult DownloadContacts([FromForm] ContactExportOptions options)
        {
            if (options.SelectedFields == null || !options.SelectedFields.Any())
            {
                TempData["error"] = "Please select at least one field to export.";
                return RedirectToAction(nameof(Index));
            }

            IQueryable<Contact> contactsQuery = _context.Contacts
                 .Include(c => c.MemberContacts).ThenInclude(mc => mc.Member)
                 .AsQueryable();

            // Filter by selected IDs if not downloading all
            if (!options.DownloadAll)
            {
                if (options.SelectedContactIds != null && options.SelectedContactIds.Any())
                {
                    contactsQuery = contactsQuery.Where(c => options.SelectedContactIds.Contains(c.ID));
                }
                else
                {
                    TempData["error"] = "No contacts selected for filtered export.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var contacts = contactsQuery.ToList();

             if (!contacts.Any())
            {
                TempData["warning"] = "No contacts found matching the selected criteria for export.";
                return RedirectToAction(nameof(Index));
            }

            // Transform data
            var exportData = new List<Dictionary<string, object>>();
            foreach (var contact in contacts)
            {
                var contactData = new Dictionary<string, object>();
                foreach (var field in options.SelectedFields)
                {
                    object value = null;
                    switch (field)
                    {
                        case "FirstName": value = contact.FirstName; break;
                        case "LastName": value = contact.LastName; break;
                        case "Email": value = contact.Email; break;
                        case "Phone": value = contact.Phone; break;
                        case "ContactType": value = contact.ContactType?.ToString(); break;
                        // Example: Get associated member names (comma-separated)
                        case "AssociatedMembers":
                            value = contact.MemberContacts != null && contact.MemberContacts.Any()
                                ? string.Join(", ", contact.MemberContacts.Select(mc => mc.Member?.MemberName ?? "N/A"))
                                : "N/A";
                            break;
                        default:
                            // Log or ignore unhandled fields
                            break;
                    }
                    contactData[field] = value;
                }
                exportData.Add(contactData);
            }

            try
            {
                byte[] fileBytes = _excelExportService.GenerateExcelPackage(
                    exportData,
                    options.SelectedFields,
                    sheetName: "Contacts",
                    reportTitle: "Contact Report"
                );

                string filename = $"Contacts_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(fileBytes, mimeType, filename);
            }
             catch (TimeZoneNotFoundException tzEx)
            {
                TempData["error"] = $"Error generating report: Timezone not found. {tzEx.Message}";
                // Log tzEx
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while generating the Excel file.";
                // Log ex
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }

        private async Task AddContactPicture(Contact contact, IFormFile thePicture)
        {
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();

                        var existingPhoto = await _context.ContactPhotos
                                                          .Where(p => p.ContactID == contact.ID)
                                                          .FirstOrDefaultAsync();
                        var existingThumbnail = await _context.ContactThumbnails
                                                              .Where(t => t.ContactID == contact.ID)
                                                              .FirstOrDefaultAsync();

                        if (existingPhoto != null)
                        {
                            // Update existing photo instead of inserting a new one
                            existingPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);
                        }
                        else
                        {
                            contact.ContactPhoto = new ContactPhoto
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                        }

                        if (existingThumbnail != null)
                        {
                            existingThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70);
                        }
                        else
                        {
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
