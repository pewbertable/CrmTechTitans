using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Data
{
    public class CrmInitializer
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().HasData(
                new Member
                {
                    Id = 1,
                    MemberName = "Tech Solutions Inc.",
                    MembershipType = MembershipType.Associate,
                    ContactedBy = "John Doe",
                    Industry = NaicsIndustryCode.ComputerAndElectronicProductManufacturing,
                    CompanySize = CompanySize.Large,
                    CompanyWebsite = "https://www.techsolutions.com",
                    MemberSince = new DateTime(2018, 5, 12),
                    LastContactDate = new DateTime(2024, 1, 10),
                    Notes = "Key partner for software development.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 2,
                    MemberName = "Energy Corp",
                    MembershipType = MembershipType.Localindustrial,
                    ContactedBy = "Jane Smith",
                    Industry = NaicsIndustryCode.OilAndGasExtraction,
                    CompanySize = CompanySize.Enterprise,
                    CompanyWebsite = "https://www.energycorp.com",
                    MemberSince = new DateTime(2015, 8, 25),
                    LastContactDate = new DateTime(2023, 11, 20),
                    Notes = "Major supplier of renewable energy.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 3,
                    MemberName = "Green Innovations Ltd.",
                    MembershipType = MembershipType.ChemberAssociate,
                    ContactedBy = "Albert Green",
                    Industry = NaicsIndustryCode.ChemicalManufacturing,
                    CompanySize = CompanySize.Medium,
                    CompanyWebsite = "https://www.greeninnovations.com",
                    MemberSince = new DateTime(2017, 3, 10),
                    LastContactDate = new DateTime(2024, 1, 5),
                    Notes = "Innovators in green technology solutions.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 4,
                    MemberName = "City Logistics",
                    MembershipType = MembershipType.Localindustrial,
                    ContactedBy = "Maria Johnson",
                    Industry = NaicsIndustryCode.TruckTransportation,
                    CompanySize = CompanySize.Small,
                    CompanyWebsite = "https://www.citylogistics.com",
                    MemberSince = new DateTime(2020, 7, 18),
                    LastContactDate = new DateTime(2023, 12, 22),
                    Notes = "Local courier and delivery services.",
                    MembershipStatus = MembershipStatus.Inactive
                },
                new Member
                {
                    Id = 5,
                    MemberName = "HealthCare Plus",
                    MembershipType = MembershipType.GovernmentEducationAssociate,
                    ContactedBy = "Emily Wright",
                    Industry = NaicsIndustryCode.MotorVehicleAndPartsDealers,
                    CompanySize = CompanySize.Enterprise,
                    CompanyWebsite = "https://www.healthcareplus.com",
                    MemberSince = new DateTime(2012, 2, 6),
                    LastContactDate = new DateTime(2023, 10, 30),
                    Notes = "Leading healthcare services provider.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 6,
                    MemberName = "Tech Electronics",
                    MembershipType = MembershipType.NonLocalIndustrial,
                    ContactedBy = "James Carter",
                    Industry = NaicsIndustryCode.ElectricalEquipmentApplianceAndComponentManufacturing,
                    CompanySize = CompanySize.Large,
                    CompanyWebsite = "https://www.techelectronics.com",
                    MemberSince = new DateTime(2016, 9, 14),
                    LastContactDate = new DateTime(2024, 1, 12),
                    Notes = "Global leader in electronic components.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 7,
                    MemberName = "Auto Parts Co.",
                    MembershipType = MembershipType.Associate,
                    ContactedBy = "Daniel Lee",
                    Industry = NaicsIndustryCode.MotorVehicleAndPartsDealers,
                    CompanySize = CompanySize.Medium,
                    CompanyWebsite = "https://www.autopartsco.com",
                    MemberSince = new DateTime(2019, 6, 22),
                    LastContactDate = new DateTime(2024, 1, 8),
                    Notes = "Supplier of auto parts to regional stores.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 8,
                    MemberName = "Foodies Market",
                    MembershipType = MembershipType.ChemberAssociate,
                    ContactedBy = "Linda Miller",
                    Industry = NaicsIndustryCode.FoodManufacturing,
                    CompanySize = CompanySize.Small,
                    CompanyWebsite = "https://www.foodiesmarket.com",
                    MemberSince = new DateTime(2021, 4, 4),
                    LastContactDate = new DateTime(2023, 12, 15),
                    Notes = "Specializing in local organic food products.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 9,
                    MemberName = "Global Transport Ltd.",
                    MembershipType = MembershipType.Localindustrial,
                    ContactedBy = "Nina Roberts",
                    Industry = NaicsIndustryCode.WaterTransportation,
                    CompanySize = CompanySize.Enterprise,
                    CompanyWebsite = "https://www.globaltransport.com",
                    MemberSince = new DateTime(2014, 10, 19),
                    LastContactDate = new DateTime(2023, 11, 30),
                    Notes = "Leading provider of international shipping solutions.",
                    MembershipStatus = MembershipStatus.Active
                },
                new Member
                {
                    Id = 10,
                    MemberName = "Universal Tech Hub",
                    MembershipType = MembershipType.Associate,
                    ContactedBy = "Oliver Thomas",
                    Industry = NaicsIndustryCode.DataProcessingHostingAndRelatedServices,
                    CompanySize = CompanySize.Large,
                    CompanyWebsite = "https://www.universaltechhub.com",
                    MemberSince = new DateTime(2013, 1, 9),
                    LastContactDate = new DateTime(2023, 12, 10),
                    Notes = "Cloud services and data hosting provider.",
                    MembershipStatus = MembershipStatus.Active
                }
            );
        }
    }
}
