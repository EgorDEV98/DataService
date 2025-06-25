using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexScheduler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Schedulers_Exchange_StartTime_EndTime",
                table: "Schedulers",
                columns: new[] { "Exchange", "StartTime", "EndTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedulers_Exchange_StartTime_EndTime",
                table: "Schedulers");
        }
    }
}
