using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataService.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Candles_ShareId",
                table: "Candles");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_Ticker",
                table: "Shares",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candles_ShareId_Interval_Time",
                table: "Candles",
                columns: new[] { "ShareId", "Interval", "Time" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shares_Ticker",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Candles_ShareId_Interval_Time",
                table: "Candles");

            migrationBuilder.CreateIndex(
                name: "IX_Candles_ShareId",
                table: "Candles",
                column: "ShareId");
        }
    }
}
