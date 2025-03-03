using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTechTitans.Data.CrmMigrations
{
    /// <inheritdoc />
    public partial class _0224 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Person",
                table: "Interactions");

            migrationBuilder.DropColumn(
                name: "interaction",
                table: "Interactions");

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Interactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InteractionDetails",
                table: "Interactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Interactions_ContactId",
                table: "Interactions",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interactions_Contacts_ContactId",
                table: "Interactions",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interactions_Contacts_ContactId",
                table: "Interactions");

            migrationBuilder.DropIndex(
                name: "IX_Interactions_ContactId",
                table: "Interactions");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Interactions");

            migrationBuilder.DropColumn(
                name: "InteractionDetails",
                table: "Interactions");

            migrationBuilder.AddColumn<string>(
                name: "Person",
                table: "Interactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "interaction",
                table: "Interactions",
                type: "TEXT",
                nullable: true);
        }
    }
}
