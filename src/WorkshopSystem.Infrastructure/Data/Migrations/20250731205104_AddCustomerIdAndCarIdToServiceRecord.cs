using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIdAndCarIdToServiceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "ServiceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "ServiceRecords",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_CarId",
                table: "ServiceRecords",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_CustomerId",
                table: "ServiceRecords",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_AspNetUsers_CustomerId",
                table: "ServiceRecords",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Cars_CarId",
                table: "ServiceRecords",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_AspNetUsers_CustomerId",
                table: "ServiceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Cars_CarId",
                table: "ServiceRecords");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRecords_CarId",
                table: "ServiceRecords");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRecords_CustomerId",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ServiceRecords");
        }
    }
}
