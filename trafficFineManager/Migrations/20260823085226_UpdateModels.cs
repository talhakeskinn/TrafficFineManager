using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace trafficFineManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Vehicles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerTC",
                table: "Vehicles",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FineTypeId",
                table: "TrafficFines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorName",
                table: "TrafficFines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ViolatorTC",
                table: "TrafficFines",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FineTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FineTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Fiat" },
                    { 4, "Toyota" },
                    { 5, "Volkswagen" }
                });

            migrationBuilder.InsertData(
                table: "FineTypes",
                columns: new[] { "Id", "Amount", "ArticleNumber", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, 1506m, "47/1-b", "Kırmızı ışık kuralına uymamak", true },
                    { 2, 1506m, "51/2-a", "Hız sınırını %10 - %30 aşmak", true },
                    { 3, 3135m, "51/2-b", "Hız sınırını %30 - %50 aşmak", true },
                    { 4, 6439m, "51/2-c", "Hız sınırını %50'den fazla aşmak", true },
                    { 5, 690m, "78/1-a", "Emniyet kemeri takmamak", true },
                    { 6, 1506m, "73/c", "Seyir halinde cep telefonu kullanmak", true }
                });

            migrationBuilder.InsertData(
                table: "Models",
                columns: new[] { "Id", "BrandId", "Name" },
                values: new object[] { 4, 2, "Focus" });

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OwnerName", "OwnerTC", "VehicleType" },
                values: new object[] { "Ahmet Yılmaz", "11111111110", 1 });

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OwnerName", "OwnerTC", "VehicleType" },
                values: new object[] { "ABC Rent A Car", "22222222220", 4 });

            migrationBuilder.InsertData(
                table: "Models",
                columns: new[] { "Id", "BrandId", "Name" },
                values: new object[,]
                {
                    { 5, 3, "Egea" },
                    { 6, 4, "Corolla" },
                    { 7, 5, "Passat" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "BrandId", "IsActive", "ModelId", "OwnerName", "OwnerTC", "PlateNumber", "VehicleType" },
                values: new object[] { 3, 3, true, 5, "Mehmet Demir", "33333333330", "35DEF456", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_FineTypeId",
                table: "TrafficFines",
                column: "FineTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficFines_FineTypes_FineTypeId",
                table: "TrafficFines",
                column: "FineTypeId",
                principalTable: "FineTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrafficFines_FineTypes_FineTypeId",
                table: "TrafficFines");

            migrationBuilder.DropTable(
                name: "FineTypes");

            migrationBuilder.DropIndex(
                name: "IX_TrafficFines_FineTypeId",
                table: "TrafficFines");

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "OwnerTC",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FineTypeId",
                table: "TrafficFines");

            migrationBuilder.DropColumn(
                name: "ViolatorName",
                table: "TrafficFines");

            migrationBuilder.DropColumn(
                name: "ViolatorTC",
                table: "TrafficFines");

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                column: "VehicleType",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2,
                column: "VehicleType",
                value: 0);
        }
    }
}
