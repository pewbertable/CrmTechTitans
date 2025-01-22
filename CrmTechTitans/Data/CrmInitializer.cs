using CRM.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
{
    public class CrmInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new CrmContext(serviceProvider.GetRequiredService<DbContextOptions<CrmContext>>()))
            {
                try
                {
                    if (!context.Industries.Any())
                    {

                        context.Industries.AddRange(
                                new Industry { Name = "Oil and Gas Extraction", NAICS = 211 },
                                new Industry { Name = "Mining and Quarrying (Except Oil and Gas)", NAICS = 212 },
                                new Industry { Name = "Support Activities for Mining and Oil and Gas Extraction", NAICS = 213 },
                                new Industry { Name = "Utilities", NAICS = 221 },
                                new Industry { Name = "Construction of Buildings", NAICS = 236 },
                                new Industry { Name = "Heavy and Civil Engineering Construction", NAICS = 237 },
                                new Industry { Name = "Specialty Trade Contractors", NAICS = 238 },
                                new Industry { Name = "Food Manufacturing", NAICS = 311 },
                                new Industry { Name = "Beverage and Tobacco Product Manufacturing", NAICS = 312 },
                                new Industry { Name = "Clothing Manufacturing", NAICS = 315 },
                                new Industry { Name = "Wood Product Manufacturing", NAICS = 321 },
                                new Industry { Name = "Paper Manufacturing", NAICS = 322 },
                                new Industry { Name = "Printing and Related Support Activities", NAICS = 323 },
                                new Industry { Name = "Chemical Manufacturing", NAICS = 325 },
                                new Industry { Name = "Plastics and Rubber Products Manufacturing", NAICS = 326 },
                                new Industry { Name = "Non-Metallic Mineral Product Manufacturing", NAICS = 327 },
                                new Industry { Name = "Primary Metal Manufacturing", NAICS = 331 },
                                new Industry { Name = "Fabricated Metal Product Manufacturing", NAICS = 332 },
                                new Industry { Name = "Machinery Manufacturing", NAICS = 333 },
                                new Industry { Name = "Computer and Electronic Product Manufacturing", NAICS = 334 },
                                new Industry { Name = "Electrical Equipment, Appliance, and Component Manufacturing", NAICS = 335 },
                                new Industry { Name = "Transportation Equipment Manufacturing", NAICS = 336 },
                                new Industry { Name = "Miscellaneous Manufacturing", NAICS = 339 },
                                new Industry { Name = "Farm Product Merchant Wholesalers", NAICS = 411 },
                                new Industry { Name = "Petroleum and Petroleum Products Merchant Wholesalers", NAICS = 412 },
                                new Industry { Name = "Building Material and Supplies Merchant Wholesalers", NAICS = 416 },
                                new Industry { Name = "Machinery, Equipment, and Supplies Merchant Wholesalers", NAICS = 417 },
                                new Industry { Name = "Miscellaneous Merchant Wholesalers", NAICS = 418 },
                                new Industry { Name = "Business to Business Electronic Markets and Agents and Brokers", NAICS = 419 },
                                new Industry { Name = "Motor Vehicle and Parts Dealers", NAICS = 441 },
                                new Industry { Name = "Building Material and Garden Equipment and Supplies Dealers", NAICS = 444 },
                                new Industry { Name = "Gasoline Stations", NAICS = 447 },
                                new Industry { Name = "Air Transportation", NAICS = 481 },
                                new Industry { Name = "Rail Transportation", NAICS = 482 },
                                new Industry { Name = "Water Transportation", NAICS = 483 },
                                new Industry { Name = "Truck Transportation", NAICS = 484 },
                                new Industry { Name = "Pipeline Transportation", NAICS = 486 },
                                new Industry { Name = "Support Activities for Transportation", NAICS = 488 },
                                new Industry { Name = "Couriers and Messengers", NAICS = 492 },
                                new Industry { Name = "Warehousing and Storage", NAICS = 493 },
                                new Industry { Name = "Telecommunications", NAICS = 517 },
                                new Industry { Name = "Data Processing, Hosting, and Related Services", NAICS = 518 },
                                new Industry { Name = "Other Information Services", NAICS = 519 },
                                new Industry { Name = "Credit Intermediation and Related Activities", NAICS = 522 },
                                new Industry { Name = "Securities, Commodity Contracts, and Other Financial Investment and Related Activities", NAICS = 523 },
                                new Industry { Name = "Insurance Carriers and Related Activities", NAICS = 524 },
                                new Industry { Name = "Funds and Other Financial Vehicles", NAICS = 526 },
                                new Industry { Name = "Real Estate", NAICS = 531 },
                                new Industry { Name = "Rental and Leasing Services", NAICS = 532 },
                                new Industry { Name = "Professional, Scientific, and Technical Services", NAICS = 541 },
                                new Industry { Name = "Management of Companies and Enterprises", NAICS = 551 },
                                new Industry { Name = "Administrative and Support Services", NAICS = 561 },
                                new Industry { Name = "Waste Management and Remediation Services", NAICS = 562 },
                                new Industry { Name = "Educational Services", NAICS = 611 },
                                new Industry { Name = "Repair and Maintenance", NAICS = 811 },
                                new Industry { Name = "Religious, Grantmaking, Civic, and Professional Organizations", NAICS = 813 },
                                new Industry { Name = "Federal Government Public Administration", NAICS = 911 },
                                new Industry { Name = "Provincial and Territorial Public Administration", NAICS = 912 },
                                new Industry { Name = "Local Municipal and Regional Public Administration", NAICS = 913 },
                                new Industry { Name = "Industrial Consulting", NAICS = 1000 },
                                new Industry { Name = "Other", NAICS = 0 });
                        context.SaveChanges();
                    }

                    if (!context.Members.Any())
                    {


                        context.Members.AddRange(

                               new Member
                               {

                                   MemberName = "Tech Solutions Inc.",
                                   MembershipType = MembershipType.Associate,
                                   ContactedBy = "John Doe",

                                   CompanySize = CompanySize.Large,
                                   CompanyWebsite = "https://www.techsolutions.com",
                                   MemberSince = new DateTime(2018, 5, 12),
                                   LastContactDate = new DateTime(2024, 1, 10),
                                   Notes = "Key partner for software development.",
                                   MembershipStatus = MembershipStatus.Active
                               },
                                new Member
                                {

                                    MemberName = "Energy Corp",
                                    MembershipType = MembershipType.Localindustrial,
                                    ContactedBy = "Jane Smith",

                                    CompanySize = CompanySize.Enterprise,
                                    CompanyWebsite = "https://www.energycorp.com",
                                    MemberSince = new DateTime(2015, 8, 25),
                                    LastContactDate = new DateTime(2023, 11, 20),
                                    Notes = "Major supplier of renewable energy.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Green Innovations Ltd.",
                                    MembershipType = MembershipType.ChamberAssociate,
                                    ContactedBy = "Albert Green",

                                    CompanySize = CompanySize.Medium,
                                    CompanyWebsite = "https://www.greeninnovations.com",
                                    MemberSince = new DateTime(2017, 3, 10),
                                    LastContactDate = new DateTime(2024, 1, 5),
                                    Notes = "Innovators in green technology solutions.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "City Logistics",
                                    MembershipType = MembershipType.Localindustrial,
                                    ContactedBy = "Maria Johnson",

                                    CompanySize = CompanySize.Small,
                                    CompanyWebsite = "https://www.citylogistics.com",
                                    MemberSince = new DateTime(2020, 7, 18),
                                    LastContactDate = new DateTime(2023, 12, 22),
                                    Notes = "Local courier and delivery services.",
                                    MembershipStatus = MembershipStatus.Inactive
                                },
                                new Member
                                {

                                    MemberName = "HealthCare Plus",
                                    MembershipType = MembershipType.GovernmentEducationAssociate,
                                    ContactedBy = "Emily Wright",

                                    CompanySize = CompanySize.Enterprise,
                                    CompanyWebsite = "https://www.healthcareplus.com",
                                    MemberSince = new DateTime(2012, 2, 6),
                                    LastContactDate = new DateTime(2023, 10, 30),
                                    Notes = "Leading healthcare services provider.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Tech Electronics",
                                    MembershipType = MembershipType.NonLocalIndustrial,
                                    ContactedBy = "James Carter",

                                    CompanySize = CompanySize.Large,
                                    CompanyWebsite = "https://www.techelectronics.com",
                                    MemberSince = new DateTime(2016, 9, 14),
                                    LastContactDate = new DateTime(2024, 1, 12),
                                    Notes = "Global leader in electronic components.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Auto Parts Co.",
                                    MembershipType = MembershipType.Associate,
                                    ContactedBy = "Daniel Lee",

                                    CompanySize = CompanySize.Medium,
                                    CompanyWebsite = "https://www.autopartsco.com",
                                    MemberSince = new DateTime(2019, 6, 22),
                                    LastContactDate = new DateTime(2024, 1, 8),
                                    Notes = "Supplier of auto parts to regional stores.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Foodies Market",
                                    MembershipType = MembershipType.ChamberAssociate,
                                    ContactedBy = "Linda Miller",

                                    CompanySize = CompanySize.Small,
                                    CompanyWebsite = "https://www.foodiesmarket.com",
                                    MemberSince = new DateTime(2021, 4, 4),
                                    LastContactDate = new DateTime(2023, 12, 15),
                                    Notes = "Specializing in local organic food products.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Global Transport Ltd.",
                                    MembershipType = MembershipType.Localindustrial,
                                    ContactedBy = "Nina Roberts",

                                    CompanySize = CompanySize.Enterprise,
                                    CompanyWebsite = "https://www.globaltransport.com",
                                    MemberSince = new DateTime(2014, 10, 19),
                                    LastContactDate = new DateTime(2023, 11, 30),
                                    Notes = "Leading provider of international shipping solutions.",
                                    MembershipStatus = MembershipStatus.Active
                                },
                                new Member
                                {

                                    MemberName = "Universal Tech Hub",
                                    MembershipType = MembershipType.Associate,
                                    ContactedBy = "Oliver Thomas",

                                    CompanySize = CompanySize.Large,
                                    CompanyWebsite = "https://www.universaltechhub.com",
                                    MemberSince = new DateTime(2013, 1, 9),
                                    LastContactDate = new DateTime(2023, 12, 10),
                                    Notes = "Cloud services and data hosting provider.",
                                    MembershipStatus = MembershipStatus.Active
                                }

                        );
                        context.SaveChanges();

                    }
                    if (!context.Members.Any())
                    {
                        context.IndustryMembers.AddRange(
                            new IndustryMember { IndustryID = 1, MemberID = 1 },
                            new IndustryMember { IndustryID = 2, MemberID = 2 },
                            new IndustryMember { IndustryID = 2, MemberID = 3 },
                             new IndustryMember { IndustryID = 1, MemberID = 4 },
                            new IndustryMember { IndustryID = 2, MemberID = 5 },
                            new IndustryMember { IndustryID = 2, MemberID = 6 },
                             new IndustryMember { IndustryID = 1, MemberID = 7 },
                            new IndustryMember { IndustryID = 2, MemberID = 8 },
                            new IndustryMember { IndustryID = 2, MemberID = 9 },
                             new IndustryMember { IndustryID = 2, MemberID = 10 }
                        ); context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }


        }
    }
}
