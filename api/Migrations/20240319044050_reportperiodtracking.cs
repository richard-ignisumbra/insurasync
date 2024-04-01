using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace permaAPI.Migrations
{
    /// <inheritdoc />
    public partial class reportperiodtracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodMonth",
                table: "Application",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodQuarter",
                table: "Application",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodMonth",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "PeriodQuarter",
                table: "Application");
        }
    }
}
