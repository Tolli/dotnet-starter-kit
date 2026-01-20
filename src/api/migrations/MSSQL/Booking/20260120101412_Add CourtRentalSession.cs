using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.MSSQL.Booking
{
    /// <inheritdoc />
    public partial class AddCourtRentalSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourtRentalSessions",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourtRentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Court = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtRentalSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtRentalSessions_CourtRentals_CourtRentalId",
                        column: x => x.CourtRentalId,
                        principalSchema: "booking",
                        principalTable: "CourtRentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourtRentalSessions_Court_StartDate_EndDate",
                schema: "booking",
                table: "CourtRentalSessions",
                columns: new[] { "Court", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtRentalSessions_CourtRentalId",
                schema: "booking",
                table: "CourtRentalSessions",
                column: "CourtRentalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourtRentalSessions",
                schema: "booking");
        }
    }
}
