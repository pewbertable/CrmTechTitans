using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTechTitans.Data.CrmMigrations
{
    /// <inheritdoc />
    public partial class CrmMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactPhotos_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactThumbnails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactThumbnails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactThumbnails_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MemberPhotos_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberThumbnails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberThumbnails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MemberThumbnails_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactPhotos_ContactID",
                table: "ContactPhotos",
                column: "ContactID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactThumbnails_ContactID",
                table: "ContactThumbnails",
                column: "ContactID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberPhotos_MemberID",
                table: "MemberPhotos",
                column: "MemberID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberThumbnails_MemberID",
                table: "MemberThumbnails",
                column: "MemberID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactPhotos");

            migrationBuilder.DropTable(
                name: "ContactThumbnails");

            migrationBuilder.DropTable(
                name: "MemberPhotos");

            migrationBuilder.DropTable(
                name: "MemberThumbnails");
        }
    }
}
