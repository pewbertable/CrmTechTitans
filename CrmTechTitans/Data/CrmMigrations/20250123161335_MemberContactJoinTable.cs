using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTechTitans.Data.CrmMigrations
{
    /// <inheritdoc />
    public partial class MemberContactJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Members",
                newName: "ID");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Opportunities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MemberAddresses",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAddresses", x => new { x.MemberId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberContacts",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberContacts", x => new { x.ContactID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_MemberContacts_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberContacts_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberAddresses_AddressId",
                table: "MemberAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberContacts_MemberID",
                table: "MemberContacts",
                column: "MemberID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberAddresses");

            migrationBuilder.DropTable(
                name: "MemberContacts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Opportunities");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Members",
                newName: "Id");
        }
    }
}
