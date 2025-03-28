using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTechTitans.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RegistrationComplete",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationComplete",
                table: "AspNetUsers");
        }
    }
}
