using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.MSSQL.Booking
{
    /// <inheritdoc />
    public partial class AddCourtrentalindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Weekday",
                schema: "booking",
                table: "CourtRentals",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CourtRentals_Weekday_StartTime_Court_StartDate_EndDate",
                schema: "booking",
                table: "CourtRentals",
                columns: new[] { "Weekday", "StartTime", "Court", "StartDate", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourtRentals_Weekday_StartTime_Court_StartDate_EndDate",
                schema: "booking",
                table: "CourtRentals");

            migrationBuilder.AlterColumn<string>(
                name: "Weekday",
                schema: "booking",
                table: "CourtRentals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
