using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceApp.Infrastructure.Migrations
{
    public partial class add_notification_audience : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Audience",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE Notifications
                SET Audience = CASE
                    WHEN ReceiverRole IS NOT NULL THEN 2
                    ELSE 1
                END
                """);

            migrationBuilder.DropColumn(
                name: "ReceiverRole",
                table: "Notifications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverRole",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Notifications
                SET ReceiverRole = CASE
                    WHEN Audience = 2 THEN 'Admin'
                    ELSE NULL
                END
                """);

            migrationBuilder.DropColumn(
                name: "Audience",
                table: "Notifications");
        }
    }
}
