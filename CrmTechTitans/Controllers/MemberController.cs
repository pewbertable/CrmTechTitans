using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using CrmTechTitans.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
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
                .Include(m => m.IndustryMembers)
                    .ThenInclude(im => im.Industry)
                .AsQueryable();

            // Filter by Member Name
            if (!string.IsNullOrEmpty(searchString))
            {
                members = members.Where(m => m.MemberName.Contains(searchString));
            }

            // Filter by Member Status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var status = Enum.Parse<MembershipStatus>(statusFilter);
                members = members.Where(m => m.MembershipStatus == status);
            }

            // Sorting
            switch (sortField)
            {
                case "MemberName":
                    members = sortDirection == "asc" ? members.OrderBy(m => m.MemberName) : members.OrderByDescending(m => m.MemberName);
                    break;
                case "MembershipStatus":
                    members = sortDirection == "asc" ? members.OrderBy(m => m.MembershipStatus) : members.OrderByDescending(m => m.MembershipStatus);
                    break;
                default:
                    members = members.OrderBy(m => m.MemberName); // Default sorting
                    break;
            }

            // Pass sorting and filtering data to the view
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["SearchString"] = searchString;
            ViewData["statusFilter"] = statusFilter;

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
                .Include(m => m.MemberContacts)
                    .ThenInclude(mc => mc.Contact)
                .Include(m => m.MemberAddresses)
                    .ThenInclude(ma => ma.Address)
                .Include(m => m.IndustryMembers)
                    .ThenInclude(im => im.Industry)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Member/Create
        public IActionResult Create()
        {
            var model = new MemberCreateViewModel
            {
                Contacts = new List<ContactViewModel> { new ContactViewModel() }, // Add one empty contact
                Addresses = new List<AddressViewModel> { new AddressViewModel() }, // Add one empty address
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
        public async Task<IActionResult> Create(MemberCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create Member
                var member = new Member
                {
                    MemberName = model.MemberName,
                    MembershipType = model.MembershipType,
                    ContactedBy = model.ContactedBy,
                    CompanySize = model.CompanySize,
                    CompanyWebsite = model.CompanyWebsite,
                    MemberSince = model.MemberSince,
                    LastContactDate = model.LastContactDate,
                    Notes = model.Notes
                };

                // Add Addresses
                foreach (var addressModel in model.Addresses)
                {
                    var address = new Address
                    {
                        Street = addressModel.Street,
                        City = addressModel.City,
                        Province = addressModel.Province,
                        PostalCode = addressModel.PostalCode
                    };
                    member.MemberAddresses.Add(new MemberAddress { Address = address, AddressType = addressModel.AddressType });
                }

                // Add Contacts
                foreach (var contactModel in model.Contacts)
                {
                    var contact = new Contact
                    {
                        FirstName = contactModel.FirstName,
                        LastName = contactModel.LastName,
                        Email = contactModel.Email,
                        Phone = contactModel.Phone
                    };
                    member.MemberContacts.Add(new MemberContact { Contact = contact, ContactType = contactModel.ContactType });
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

                return RedirectToAction(nameof(Index));
            }

            // If the model is invalid, repopulate available industries
            model.AvailableIndustries = _context.Industries
            .Select(industry => new IndustryViewModel
            {
                ID = industry.ID,
                Name = industry.Name,
                NAICS = industry.NAICS
            }).ToList(); // Convert Industry entities to IndustryViewModels
            return View(model);
        }

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberContacts)
                    .ThenInclude(mc => mc.Contact)
                .Include(m => m.MemberAddresses)
                    .ThenInclude(ma => ma.Address)
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
                // Map other member properties...
                Addresses = member.MemberAddresses.Select(ma => new AddressViewModel
                {
                    Street = ma.Address.Street,
                    City = ma.Address.City,
                    Province = ma.Address.Province,
                    PostalCode = ma.Address.PostalCode,
                    AddressType = ma.AddressType // Map address type
                }).ToList(),
                Contacts = member.MemberContacts.Select(mc => new ContactViewModel
                {
                    FirstName = mc.Contact.FirstName,
                    LastName = mc.Contact.LastName,
                    Email = mc.Contact.Email,
                    Phone = mc.Contact.Phone,
                    ContactType = mc.ContactType
                }).ToList(),
                SelectedIndustryIds = member.IndustryMembers.Select(im => im.IndustryID).ToList(), // Selected industry IDs
                AvailableIndustries = _context.Industries
            .Select(industry => new IndustryViewModel
            {
                ID = industry.ID,
                Name = industry.Name,
                NAICS = industry.NAICS
            }).ToList() // Convert Industry entities to IndustryViewModels // Populate available industries
            };

            return View(model);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberCreateViewModel model)
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
                        .Include(m => m.MemberContacts)
                        .Include(m => m.MemberAddresses)
                        .Include(m => m.IndustryMembers)
                        .FirstOrDefaultAsync(m => m.ID == id);

                    if (member == null)
                    {
                        return NotFound();
                    }

                    // Update Member properties
                    member.MemberName = model.MemberName;
                    // Update other member properties...

                    // Update Addresses
                    member.MemberAddresses.Clear();
                    foreach (var addressModel in model.Addresses)
                    {
                        var address = new Address
                        {
                            Street = addressModel.Street,
                            City = addressModel.City,
                            Province = addressModel.Province,
                            PostalCode = addressModel.PostalCode
                        };
                        member.MemberAddresses.Add(new MemberAddress { Address = address, AddressType = addressModel.AddressType });
                    }

                    // Update Contacts
                    member.MemberContacts.Clear();
                    foreach (var contactModel in model.Contacts)
                    {
                        var contact = new Contact
                        {
                            FirstName = contactModel.FirstName,
                            LastName = contactModel.LastName,
                            Email = contactModel.Email,
                            Phone = contactModel.Phone
                        };
                        member.MemberContacts.Add(new MemberContact { Contact = contact, ContactType = contactModel.ContactType });
                    }

                    // Update Industries
                    member.IndustryMembers.Clear();
                    foreach (var industryId in model.SelectedIndustryIds)
                    {
                        var industry = await _context.Industries.FindAsync(industryId);
                        if (industry != null)
                        {
                            member.IndustryMembers.Add(new MemberIndustry { Industry = industry });
                        }
                    }

                    // Save changes
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Member edited successfully";

                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["errMessage"] = "An error occured. Failed to edit the member.";
                    if (!MemberExists(id))
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

            // If the model is invalid, repopulate available industries
            model.AvailableIndustries = _context.Industries
            .Select(industry => new IndustryViewModel
            {
                ID = industry.ID,
                Name = industry.Name,
                NAICS = industry.NAICS
            }).ToList(); // Convert Industry entities to IndustryViewModels
            return View(model);
        }

        // GET: Member/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }
    }
}