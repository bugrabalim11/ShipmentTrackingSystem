using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracking.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserIdToShipmnet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Shipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_AppUserId",
                table: "Shipments",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_AppUsers_AppUserId",
                table: "Shipments",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_AppUsers_AppUserId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_AppUserId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Shipments");
        }
    }
}
