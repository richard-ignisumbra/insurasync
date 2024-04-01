using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace permaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationElementLabel");

            migrationBuilder.DropTable(
                name: "ApplicationElementLine");

            migrationBuilder.DropTable(
                name: "ApplicationElementResponse")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ApplicationElementResponse_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "EndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "StartTime");

            migrationBuilder.DropTable(
                name: "ApplicationElementType");

            migrationBuilder.DropTable(
                name: "ApplicationReport");

            migrationBuilder.DropTable(
                name: "ApplicationSectionApplicationType");

            migrationBuilder.DropTable(
                name: "ApplicationSectionElementLineofCoverage");

            migrationBuilder.DropTable(
                name: "ApplicationSectionLineofCoverage");

            migrationBuilder.DropTable(
                name: "ApplicationSectionLinesofCoverage");

            migrationBuilder.DropTable(
                name: "ApplicationSectionResponse");

            migrationBuilder.DropTable(
                name: "ApplicationTableDefaultRow");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "AttachmentCategory");

            migrationBuilder.DropTable(
                name: "ConfigurationSettings");

            migrationBuilder.DropTable(
                name: "ContactPermissionType");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "emailTemplate");

            migrationBuilder.DropTable(
                name: "ExportCategory");

            migrationBuilder.DropTable(
                name: "InsuredContact");

            migrationBuilder.DropTable(
                name: "InsuredLineofCoverage");

            migrationBuilder.DropTable(
                name: "InsuredNote");

            migrationBuilder.DropTable(
                name: "NoteCategory");

            migrationBuilder.DropTable(
                name: "ReportType");

            migrationBuilder.DropTable(
                name: "ApplicationSectionElement");

            migrationBuilder.DropTable(
                name: "ApplicationType");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "ApplicationSection");

            migrationBuilder.DropTable(
                name: "permissionType");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "LineofCoverage");

            migrationBuilder.DropTable(
                name: "Insured");
        }
    }
}
