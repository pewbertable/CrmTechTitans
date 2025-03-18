﻿using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using CrmTechTitans.Models.ViewModels;
using CrmTechTitans.Utilities;
using CrmTechTitans.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly CrmContext _context;

        public MemberController(CrmContext context)
        {
            _context = context;
        }

        // GET: Member
        public async Task<IActionResult> Index(string searchString, string statusFilter, string sortField, string sortDirection)
        {
            // Start with the base query
            var members = _context.Members
                .Include(m => m.MemberThumbnail)
                .Include(m => m.IndustryMembers)
                .ThenInclude(im => im.Industry)
                .Include(m => m.MemberMembershipTypes) // Include the join table
                .ThenInclude(mmt => mmt.MembershipType)
                .AsQueryable();



            return View(await members.ToListAsync());
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                 .Include(m => m.MemberPhoto)
                 .Include(m => m.MemberContacts)
                     .ThenInclude(mc => mc.Contact)
                 .Include(m => m.MemberAddresses)
                     .ThenInclude(ma => ma.Address)
                 .Include(m => m.IndustryMembers)
                     .ThenInclude(im => im.Industry)
                 .Include(m => m.MemberMembershipTypes)
                     .ThenInclude(mmt => mmt.MembershipType)
                 .FirstOrDefaultAsync(m => m.ID == id);


            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult ToggleArchive([FromBody] MembershipStatusUpdateVM model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NewStatus))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            var member = _context.Members.Find(model.MemberId);
            if (member == null)
            {
                return NotFound(new { message = "Member not found." });
            }

            Console.WriteLine($"Received MemberId: {model.MemberId}, NewStatus: {model.NewStatus}");

            if (!Enum.TryParse(model.NewStatus, true, out MembershipStatus newStatus))
            {
                return BadRequest(new { message = "Invalid membership status." });
            }

            // Update and save status
            member.MembershipStatus = newStatus;
            _context.SaveChanges();

            return Ok(new { message = $"Member status updated to {newStatus}" });
        }


        // GET: Member/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create()
        {
            var model = new MemberCreateViewModel
            {
                Contacts = new List<ContactViewModel> { new ContactViewModel() }, // Add one empty contact
                Addresses = new List<AddressViewModel> { new AddressViewModel() }, // Add one empty address

                // Load Available Membership Types
                AvailableMembershipTypes = _context.MembershipTypes
            .Select(m => new MembershipTypeViewModel
            {
                ID = m.ID,
                Name = m.Name
            }).ToList(),

                AvailableIndustries = _context.Industries
            .Select(industry => new IndustryViewModel
            {
                ID = industry.ID,
                Name = industry.Name,
                NAICS = industry.NAICS
            }).ToList() // Convert Industry entities to IndustryViewModels
            };

            return View(model);
        }

        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create(MemberCreateViewModel model, IFormFile? memberPicture, IFormFile? contactPicture)
        {
            // Validate required collections
            if (model.SelectedMembershipTypeIDs == null || !model.SelectedMembershipTypeIDs.Any())
            {
                ModelState.AddModelError("SelectedMembershipTypeIDs", "At least one membership type is required");
            }

            if (model.SelectedIndustryIds == null || !model.SelectedIndustryIds.Any())
            {
                ModelState.AddModelError("SelectedIndustryIds", "At least one industry is required");
            }

            // Validate addresses
            bool hasValidAddress = false;
            if (model.Addresses != null && model.Addresses.Any())
            {
                foreach (var address in model.Addresses)
                {
                    if (!string.IsNullOrWhiteSpace(address.Street) && 
                        !string.IsNullOrWhiteSpace(address.City) && 
                        address.Province != 0)
                    {
                        hasValidAddress = true;
                        break;
                    }
                }
            }

            if (!hasValidAddress)
            {
                ModelState.AddModelError("Addresses", "At least one complete address is required");
            }

            // Validate contacts
            bool hasValidContact = false;
            if (model.Contacts != null && model.Contacts.Any())
            {
                foreach (var contact in model.Contacts)
                {
                    if (!string.IsNullOrWhiteSpace(contact.FirstName) && 
                        !string.IsNullOrWhiteSpace(contact.Phone))
                    {
                        hasValidContact = true;
                        break;
                    }
                }
            }

            if (!hasValidContact)
            {
                ModelState.AddModelError("Contacts", "At least one contact with name and phone is required");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await AddMemberPicture(model, memberPicture);
                    // Create Member
                    var member = new Member
                    {
                        MemberName = model.MemberName,
                        ContactedBy = model.ContactedBy,
                        CompanySize = model.CompanySize,
                        CompanyWebsite = model.CompanyWebsite,
                        MemberSince = model.MemberSince,
                        LastContactDate = model.LastContactDate,
                        Notes = model.Notes,
                        MemberPhoto = model.MemberPhoto,
                        MemberThumbnail = model.MemberThumbnail
                    };

                    // Add Addresses
                    foreach (var addressModel in model.Addresses)
                    {
                        // Skip empty addresses
                        if (string.IsNullOrWhiteSpace(addressModel.Street) && 
                            string.IsNullOrWhiteSpace(addressModel.City))
                        {
                            continue;
                        }

                        var address = new Address
                        {
                            Street = addressModel.Street,
                            City = addressModel.City,
                            Province = addressModel.Province,
                            PostalCode = addressModel.PostalCode,
                            AddressType = addressModel.AddressType
                        };
                        member.MemberAddresses.Add(new MemberAddress { Address = address });
                    }

                    // Assign multiple membership types
                    foreach (var membershipTypeID in model.SelectedMembershipTypeIDs)
                    {
                        member.MemberMembershipTypes.Add(new MemberMembershipType
                        {
                            MembershipTypeID = membershipTypeID
                        });
                    }

                    // Add Contacts
                    foreach (var contactModel in model.Contacts)
                    {
                        // Skip empty contacts
                        if (string.IsNullOrWhiteSpace(contactModel.FirstName) && 
                            string.IsNullOrWhiteSpace(contactModel.Phone))
                        {
                            continue;
                        }

                        await AddContactPicture(contactModel, contactPicture);
                        var contact = new Contact
                        {
                            FirstName = contactModel.FirstName,
                            LastName = contactModel.LastName,
                            Email = contactModel.Email,
                            Phone = contactModel.Phone,
                            ContactType = contactModel.ContactType,
                            ContactPhoto = contactModel.ContactPhoto,
                            ContactThumbnail = contactModel.ContactThumbnail
                        };
                        member.MemberContacts.Add(new MemberContact { Contact = contact });
                    }

                    // Add Selected Industries
                    foreach (var industryId in model.SelectedIndustryIds)
                    {
                        var industry = await _context.Industries.FindAsync(industryId);
                        if (industry != null)
                        {
                            member.IndustryMembers.Add(new MemberIndustry { Industry = industry });
                        }
                    }

                    // Save to Database
                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();
                    
                    TempData["success"] = "Member created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating member: {ex.Message}");
                    // Log the exception
                    Console.WriteLine($"Error creating member: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors for debugging
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                }
            }

            // If we get here, something failed, redisplay form
            // Repopulate available industries
            model.AvailableIndustries = _context.Industries
                .Select(industry => new IndustryViewModel
                {
                    ID = industry.ID,
                    Name = industry.Name,
                    NAICS = industry.NAICS
                }).ToList();

            // Reload membership types
            model.AvailableMembershipTypes = _context.MembershipTypes
                .Select(m => new MembershipTypeViewModel
                {
                    ID = m.ID,
                    Name = m.Name
                }).ToList();
                
            // Add a flag to indicate validation errors
            ViewBag.HasValidationErrors = true;
            
            // Store the current step in TempData to help the client-side script restore the correct step
            int currentStep = 0;
            
            // Determine which step has validation errors
            if (ModelState["MemberName"]?.Errors.Count > 0 || 
                ModelState["CompanySize"]?.Errors.Count > 0 || 
                ModelState["CompanyWebsite"]?.Errors.Count > 0 || 
                ModelState["ContactedBy"]?.Errors.Count > 0 ||
                ModelState["SelectedMembershipTypeIDs"]?.Errors.Count > 0)
            {
                currentStep = 0; // Step 1 has errors
            }
            else if (ModelState["MemberSince"]?.Errors.Count > 0 || 
                     ModelState["LastContactDate"]?.Errors.Count > 0 || 
                     ModelState["Notes"]?.Errors.Count > 0)
            {
                currentStep = 1; // Step 2 has errors
            }
            else if (ModelState["Addresses"]?.Errors.Count > 0)
            {
                currentStep = 2; // Step 3 has errors
            }
            else if (ModelState["Contacts"]?.Errors.Count > 0 || 
                     ModelState["SelectedIndustryIds"]?.Errors.Count > 0)
            {
                currentStep = 3; // Step 4 has errors
            }
            
            // Store the current step in session
            HttpContext.Session.SetInt32("currentFormStep", currentStep);
            
            return View(model);
        }

        //GET Edit
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberPhoto)
                .Include(m => m.MemberThumbnail)
                .Include(m => m.MemberContacts)
                .ThenInclude(mc => mc.Contact)
                .Include(m => m.MemberMembershipTypes) // Include the join table
                .ThenInclude(mmt => mmt.MembershipType)
                .Include(m => m.MemberAddresses) // Ensure we include addresses
                .ThenInclude(ma => ma.Address) // Include the Address entity
                .Include(m => m.IndustryMembers)
                .ThenInclude(im => im.Industry)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member == null)
            {
                return NotFound();
            }

            // Map Member to ViewModel
            var model = new MemberCreateViewModel
            {
                ID = member.ID,
                MemberName = member.MemberName,
                ContactedBy = member.ContactedBy,
                CompanySize = member.CompanySize,
                CompanyWebsite = member.CompanyWebsite,
                MemberSince = member.MemberSince,
                LastContactDate = member.LastContactDate,
                Notes = member.Notes,
                MembershipStatus = member.MembershipStatus,
                MemberPhoto = member.MemberPhoto,
                MemberThumbnail = member.MemberThumbnail,
                Addresses = member.MemberAddresses.Select(ma => new AddressViewModel
                {
                    ID = ma.Address.ID, // Ensure ID is mapped
                    Street = ma.Address.Street,
                    City = ma.Address.City,
                    Province = ma.Address.Province,
                    PostalCode = ma.Address.PostalCode,
                    AddressType = ma.AddressType // Map address type
                }).ToList(),
                Contacts = member.MemberContacts.Select(mc => new ContactViewModel
                {
                    ID = mc.Contact.ID, // Ensure ID is included
                    FirstName = mc.Contact.FirstName,
                    LastName = mc.Contact.LastName,
                    Email = mc.Contact.Email,
                    Phone = mc.Contact.Phone,
                    ContactType = mc.ContactType,
                    ContactPhoto = mc.Contact.ContactPhoto,
                    ContactThumbnail = mc.Contact.ContactThumbnail
                }).ToList(),
                SelectedIndustryIds = member.IndustryMembers.Select(im => im.IndustryID).ToList(),
                SelectedMembershipTypeIDs = member.MemberMembershipTypes
                    .Select(mmt => mmt.MembershipTypeID)
                    .ToList(),
                AvailableIndustries = _context.Industries
                    .Select(industry => new IndustryViewModel
                    {
                        ID = industry.ID,
                        Name = industry.Name,
                        NAICS = industry.NAICS
                    }).ToList(),
                AvailableMembershipTypes = _context.MembershipTypes
                    .Select(mt => new MembershipTypeViewModel
                    {
                        ID = mt.ID,
                        Name = mt.Name
                    }).ToList()
            };

            return View(model);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, MemberCreateViewModel model, string? chkRemoveMemberImage, IFormFile? memberPicture)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing member from the database
                    var member = await _context.Members
                        .Include(m => m.MemberPhoto)
                        .Include(m => m.MemberThumbnail)
                        .Include(m => m.MemberMembershipTypes)
                        .Include(m => m.IndustryMembers)
                        .FirstOrDefaultAsync(m => m.ID == id);

                    if (member == null)
                    {
                        return NotFound();
                    }

                    // Update Member properties
                    member.MemberName = model.MemberName;
                    member.ContactedBy = model.ContactedBy;
                    member.CompanySize = model.CompanySize;
                    member.CompanyWebsite = model.CompanyWebsite;
                    member.MemberSince = model.MemberSince;
                    member.LastContactDate = model.LastContactDate;
                    member.Notes = model.Notes;
                    member.MembershipStatus = model.MembershipStatus;

                    // Handle Image Updates
                    if (chkRemoveMemberImage != null)
                    {
                        if (member.MemberPhoto != null)
                            _context.MemberPhotos.Remove(member.MemberPhoto);
                        if (member.MemberThumbnail != null)
                            _context.MemberThumbnails.Remove(member.MemberThumbnail);
                        member.MemberPhoto = null;
                        member.MemberThumbnail = null;
                    }
                    else
                    {
                        await AddMemberPicture(model, memberPicture);
                        if (model.MemberPhoto != null) member.MemberPhoto = model.MemberPhoto;
                        if (model.MemberThumbnail != null) member.MemberThumbnail = model.MemberThumbnail;
                    }

                    // Update Industries - clear and re-add
                    member.IndustryMembers.Clear();
                    foreach (var industryId in model.SelectedIndustryIds)
                    {
                        member.IndustryMembers.Add(new MemberIndustry { IndustryID = industryId });
                    }

                    // Update Membership Types - clear and re-add
                    member.MemberMembershipTypes.Clear();
                    foreach (var membershipTypeId in model.SelectedMembershipTypeIDs)
                    {
                        member.MemberMembershipTypes.Add(new MemberMembershipType
                        {
                            MembershipTypeID = membershipTypeId
                        });
                    }

                    // Save changes
                    _context.Update(member);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Member edited successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!MemberExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["error"] = "An error occurred. Failed to edit the member.";
                        Console.WriteLine($"Error updating member: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = "An error occurred. Failed to edit the member.";
                    Console.WriteLine($"Error updating member: {ex.Message}");
                }
            }
            
            // If we get here, something failed, redisplay form
            // Repopulate available industries and membership types
            model.AvailableIndustries = _context.Industries
                .Select(industry => new IndustryViewModel
                {
                    ID = industry.ID,
                    Name = industry.Name,
                    NAICS = industry.NAICS
                }).ToList();

            model.AvailableMembershipTypes = _context.MembershipTypes
                .Select(m => new MembershipTypeViewModel
                {
                    ID = m.ID,
                    Name = m.Name
                }).ToList();
                
            return View(model);
        }
        // GET: Member/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members
                .FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(string email, string phone, int? excludeContactId = null)
        {
            // If both email and phone are empty, there's nothing to check
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone))
                return false;

            var query = _context.Contacts.AsQueryable();

            if (excludeContactId.HasValue)
            {
                query = query.Where(c => c.ID != excludeContactId.Value);
            }

            // Check for duplicates if either email OR phone match (when they're not empty)
            return query.Any(c => 
                (!string.IsNullOrEmpty(email) && c.Email == email) || 
                (!string.IsNullOrEmpty(phone) && c.Phone == phone));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }

        private async Task AddMemberPicture(MemberCreateViewModel member, IFormFile thePicture)
        {
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0)) // Check if file exists
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray(); // Convert to byte array

                        // Retrieve existing MemberPhoto and MemberThumbnail from the database
                        var existingPhoto = await _context.MemberPhotos
                                                          .Where(p => p.MemberID == member.ID)
                                                          .FirstOrDefaultAsync();
                        var existingThumbnail = await _context.MemberThumbnails
                                                              .Where(t => t.MemberID == member.ID)
                                                              .FirstOrDefaultAsync();

                        if (existingPhoto != null)
                        {
                            // Update the existing photo instead of creating a new one
                            existingPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);
                        }
                        else
                        {
                            member.MemberPhoto = new MemberPhoto
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                        }

                        if (existingThumbnail != null)
                        {
                            // Update existing thumbnail
                            existingThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70);
                        }
                        else
                        {
                            member.MemberThumbnail = new MemberThumbnail
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }

        private async Task AddContactPicture(ContactViewModel contact, IFormFile thePicture)
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
        public IActionResult ExportMembers()
        {
            var model = new MemberExportViewModel
            {
                Members = _context.Members.ToList() // Fetch members from the database
            };
            return View(model); // Returns the ExportMembers.cshtml view
        }

        [HttpPost]
        public IActionResult DownloadMembers([FromForm] MemberExportOptions options)
        {
            if (options.SelectedFields == null || !options.SelectedFields.Any())
            {
                return BadRequest("Please select at least one field to export.");
            }

            //IQueryable<Member> membersQuery = _context.Members.AsQueryable();
            IQueryable<Member> membersQuery = _context.Members
             .Include(m => m.MemberContacts)
             .Include(m => m.IndustryMembers).ThenInclude(im => im.Industry)
             .Include(m => m.MemberAddresses).ThenInclude(a => a.Address)
             .AsQueryable();

            // If "Download All" is selected, fetch all members
            if (!options.DownloadAll)
            {
                // Otherwise, filter by selected members
                if (options.SelectedMemberIds != null && options.SelectedMemberIds.Any())
                {
                    membersQuery = membersQuery.Where(m => options.SelectedMemberIds.Contains(m.ID));
                }
                else
                {
                    return BadRequest("No members selected.");
                }
            }

            var members = membersQuery.ToList();

            // Set the license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Members");

                int column = 1;
                Dictionary<string, int> fieldMapping = new Dictionary<string, int>();

                // Add headers properly
                foreach (var field in options.SelectedFields)
                {
                    workSheet.Cells[3, column].Value = field;  // <-- Row 3 for column headings
                    workSheet.Cells[3, column].Style.Font.Bold = true;
                    fieldMapping[field] = column;
                    column++;
                }

                int row = 4; // Data starts from row 4
                foreach (var member in members)
                {
                    column = 1;
                    foreach (var field in options.SelectedFields)
                    {
                        switch (field)
                        {
                            case "MemberName":
                                workSheet.Cells[row, column].Value = member.MemberName;
                                break;
                            case "CompanySize":
                                workSheet.Cells[row, column].Value = member.CompanySize.ToString();
                                break;
                            case "Website":
                                workSheet.Cells[row, column].Value = member.CompanyWebsite;
                                break;
                            case "MembershipStatus":
                                workSheet.Cells[row, column].Value = member.MembershipStatus.ToString();
                                break;
                            case "ContactedBy":
                                workSheet.Cells[row, column].Value = member.ContactedBy.ToString();
                                break;
                            case "MemberSince":
                                workSheet.Cells[row, column].Value = member.MemberSince.ToShortDateString();
                                workSheet.Column(column).Style.Numberformat.Format = "yyyy-mm-dd";
                                break;
                            case "Notes":
                                workSheet.Cells[row, column].Value = member.Notes;
                                break;
                            case "Industries":
                                workSheet.Cells[row, column].Value = string.Join(", ",
                                    member.IndustryMembers.Select(im => im.Industry.NAICS)); // List industries
                                break;
                            case "Address":
                                workSheet.Cells[row, column].Value = member.MemberAddresses
                                    .FirstOrDefault()?.Address?.Summary ?? "N/A"; // Fetch address
                                break;
                        }
                        column++;
                    }
                    row++;
                }

                workSheet.Cells.AutoFitColumns();

                // Fix title and timestamp formatting
                workSheet.Cells[1, 1].Value = "Member Report";
                using (ExcelRange title = workSheet.Cells[1, 1, 1, options.SelectedFields.Count])
                {
                    title.Merge = true;
                    title.Style.Font.Bold = true;
                    title.Style.Font.Size = 18;
                    title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Fix "Created Date" placement (move it below title)
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                workSheet.Cells[2, 1].Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                workSheet.Cells[2, 1].Style.Font.Bold = true;
                workSheet.Cells[2, 1].Style.Font.Size = 12;

                try
                {
                    Byte[] fileBytes = excel.GetAsByteArray();
                    string filename = "Members.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(fileBytes, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not generate the file.");
                }
            }
        }

    }

}