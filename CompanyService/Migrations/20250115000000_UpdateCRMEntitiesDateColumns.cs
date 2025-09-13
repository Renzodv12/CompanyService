using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCRMEntitiesDateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename CreatedDate to CreatedAt for Campaign
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Campaigns",
                newName: "CreatedAt");

            // Rename ModifiedDate to UpdatedAt for Campaign
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Campaigns",
                newName: "UpdatedAt");

            // Rename CreatedDate to CreatedAt for Lead
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Leads",
                newName: "CreatedAt");

            // Rename ModifiedDate to UpdatedAt for Lead
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Leads",
                newName: "UpdatedAt");

            // Rename CreatedDate to CreatedAt for Opportunity
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Opportunities",
                newName: "CreatedAt");

            // Rename ModifiedDate to UpdatedAt for Opportunity
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Opportunities",
                newName: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert CreatedAt to CreatedDate for Campaign
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Campaigns",
                newName: "CreatedDate");

            // Revert UpdatedAt to ModifiedDate for Campaign
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Campaigns",
                newName: "ModifiedDate");

            // Revert CreatedAt to CreatedDate for Lead
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Leads",
                newName: "CreatedDate");

            // Revert UpdatedAt to ModifiedDate for Lead
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Leads",
                newName: "ModifiedDate");

            // Revert CreatedAt to CreatedDate for Opportunity
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Opportunities",
                newName: "CreatedDate");

            // Revert UpdatedAt to ModifiedDate for Opportunity
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Opportunities",
                newName: "ModifiedDate");
        }
    }
}