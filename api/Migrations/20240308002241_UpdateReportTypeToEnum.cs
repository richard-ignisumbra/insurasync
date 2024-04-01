using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace permaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupType",
                table: "ApplicationType",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupType",
                table: "ApplicationType");
        }
    }
}
