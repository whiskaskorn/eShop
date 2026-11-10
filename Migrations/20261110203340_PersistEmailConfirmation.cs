using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.Api.Migrations
{
    /// <inheritdoc />
    public partial class PersistEmailConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isConfirmEmail",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isConfirmEmail",
                table: "Users");
        }
    }
}
