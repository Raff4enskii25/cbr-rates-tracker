using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CbrRatesTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Alerts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Alerts");
        }
    }
}
