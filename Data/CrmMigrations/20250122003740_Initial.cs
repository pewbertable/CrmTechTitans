using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CrmTechTitans.Data.CrmMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Province = table.Column<int>(type: "INTEGER", nullable: false),
                    Zip = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "interactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    interaction = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Industry = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanySize = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyWebsite = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MemberSince = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastContactDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MembershipStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Priority = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "CompanySize", "CompanyWebsite", "ContactedBy", "Industry", "LastContactDate", "MemberName", "MemberSince", "MembershipStatus", "MembershipType", "Notes" },
                values: new object[,]
                {
                    { 1, 3, "https://www.techsolutions.com", "John Doe", 334, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tech Solutions Inc.", new DateTime(2018, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "Key partner for software development." },
                    { 2, 4, "https://www.energycorp.com", "Jane Smith", 211, new DateTime(2023, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Energy Corp", new DateTime(2015, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 3, "Major supplier of renewable energy." },
                    { 3, 2, "https://www.greeninnovations.com", "Albert Green", 325, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Green Innovations Ltd.", new DateTime(2017, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, "Innovators in green technology solutions." },
                    { 4, 1, "https://www.citylogistics.com", "Maria Johnson", 484, new DateTime(2023, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "City Logistics", new DateTime(2020, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Local courier and delivery services." },
                    { 5, 4, "https://www.healthcareplus.com", "Emily Wright", 441, new DateTime(2023, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "HealthCare Plus", new DateTime(2012, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, "Leading healthcare services provider." },
                    { 6, 3, "https://www.techelectronics.com", "James Carter", 335, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tech Electronics", new DateTime(2016, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 4, "Global leader in electronic components." },
                    { 7, 2, "https://www.autopartsco.com", "Daniel Lee", 441, new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Auto Parts Co.", new DateTime(2019, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "Supplier of auto parts to regional stores." },
                    { 8, 1, "https://www.foodiesmarket.com", "Linda Miller", 311, new DateTime(2023, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Foodies Market", new DateTime(2021, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, "Specializing in local organic food products." },
                    { 9, 4, "https://www.globaltransport.com", "Nina Roberts", 483, new DateTime(2023, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Global Transport Ltd.", new DateTime(2014, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 3, "Leading provider of international shipping solutions." },
                    { 10, 3, "https://www.universaltechhub.com", "Oliver Thomas", 518, new DateTime(2023, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Universal Tech Hub", new DateTime(2013, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "Cloud services and data hosting provider." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "interactions");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Opportunities");
        }
    }
}
