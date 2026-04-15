using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class audience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverRole",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "Audience",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audience",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverRole",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
