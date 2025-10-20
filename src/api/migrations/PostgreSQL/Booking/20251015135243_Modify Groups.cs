using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.Booking
{
    /// <inheritdoc />
    public partial class ModifyGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "booking",
                table: "Groups");

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                schema: "booking",
                table: "Groups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "booking",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "booking",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ContactId",
                schema: "booking",
                table: "Groups",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Customers_ContactId",
                schema: "booking",
                table: "Groups",
                column: "ContactId",
                principalSchema: "booking",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Customers_ContactId",
                schema: "booking",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ContactId",
                schema: "booking",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ContactId",
                schema: "booking",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "booking",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "booking",
                table: "Groups");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "booking",
                table: "Groups",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
