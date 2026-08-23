using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace trafficFineManager.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "BrandId", "IsActive", "ModelId", "PlateNumber", "VehicleType" },
                values: new object[,]
                {
                    { 1, 1, true, 1, "34ABC123", 0 },
                    { 2, 2, true, 3, "06XYZ987", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
