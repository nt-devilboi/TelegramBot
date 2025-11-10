using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingBot.Migrations
{
    /// <inheritdoc />
    public partial class ll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Chat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Chat",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Chat",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
