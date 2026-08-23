using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace trafficFineManager.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedFines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FineTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Hız sınırını %10'dan %30'a kadar aşmak");

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Hız sınırını %30'dan %50'ye kadar aşmak");

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Amount", "ArticleNumber", "Description" },
                values: new object[] { 1506.00m, "73/c", "Seyir halinde cep telefonu kullanmak" });

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Amount", "ArticleNumber", "Description" },
                values: new object[] { 6439.00m, "48/5", "Alkollü araç kullanmak (1. Defa)" });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Focus");

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Fiesta");

            migrationBuilder.InsertData(
                table: "TrafficFines",
                columns: new[] { "Id", "Amount", "CityId", "CreatedAt", "CreatorUserId", "DistrictId", "FineTypeId", "NotificationDate", "ReceiptNumber", "Status", "VehicleId", "ViolationDate", "ViolationReason", "ViolatorName", "ViolatorTC" },
                values: new object[,]
                {
                    { 1, 1506.00m, 1, new DateTime(2026, 7, 9, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, new DateTime(2026, 7, 9, 14, 0, 0, 0, DateTimeKind.Unspecified), "TR-2026-001", 4, 1, new DateTime(2026, 7, 9, 12, 0, 0, 0, DateTimeKind.Unspecified), "Kırmızı ışık kuralına uymamak", "Ali Yılmaz", "11111111110" },
                    { 2, 1506.00m, 2, new DateTime(2026, 8, 3, 13, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, 2, new DateTime(2026, 8, 3, 13, 0, 0, 0, DateTimeKind.Unspecified), "TR-2026-002", 5, 2, new DateTime(2026, 8, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), "Hız sınırını %10'dan %30'a kadar aşmak", "Ayşe Demir", "44444444440" },
                    { 3, 1506.00m, 1, new DateTime(2026, 8, 18, 12, 45, 0, 0, DateTimeKind.Unspecified), 1, 2, 4, new DateTime(2026, 8, 18, 12, 45, 0, 0, DateTimeKind.Unspecified), "TR-2026-003", 3, 1, new DateTime(2026, 8, 18, 12, 0, 0, 0, DateTimeKind.Unspecified), "Seyir halinde cep telefonu kullanmak", "Ali Yılmaz", "11111111110" },
                    { 4, 3135.00m, 2, new DateTime(2026, 8, 23, 11, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 3, new DateTime(2026, 8, 23, 11, 0, 0, 0, DateTimeKind.Unspecified), "TR-2026-004", 1, 3, new DateTime(2026, 8, 23, 10, 0, 0, 0, DateTimeKind.Unspecified), "Hız sınırını %30'dan %50'ye kadar aşmak", "Mehmet Demir", "33333333330" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "ahmet@test.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "ayse@test.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "FirstName" },
                values: new object[] { "fatma@test.com", "Fatma" });

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                column: "OwnerName",
                value: "Ali Yılmaz");

            migrationBuilder.InsertData(
                table: "TrafficFineHistories",
                columns: new[] { "Id", "ActionDate", "ActionType", "Description", "NewStatus", "OldStatus", "TrafficFineId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 9, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, "Ceza sisteme eklendi.", 1, 1, 1, 1 },
                    { 2, new DateTime(2026, 7, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, "Yönetici onayı verildi. Finans onayı bekleniyor.", 3, 1, 1, 2 },
                    { 3, new DateTime(2026, 7, 11, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, "Finans onayı verildi. İşlem kesinleşti (Tamamlandı).", 4, 3, 1, 3 },
                    { 4, new DateTime(2026, 8, 3, 13, 0, 0, 0, DateTimeKind.Unspecified), 1, "Ceza sisteme eklendi.", 1, 1, 2, 1 },
                    { 5, new DateTime(2026, 8, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), 3, "Plaka okunamıyor, kayıt reddedildi.", 5, 1, 2, 2 },
                    { 6, new DateTime(2026, 8, 18, 12, 45, 0, 0, DateTimeKind.Unspecified), 1, "Ceza sisteme eklendi.", 1, 1, 3, 1 },
                    { 7, new DateTime(2026, 8, 19, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, "Yönetici onayı verildi. Finans onayı bekleniyor.", 3, 1, 3, 2 },
                    { 8, new DateTime(2026, 8, 23, 11, 0, 0, 0, DateTimeKind.Unspecified), 1, "Ceza sisteme eklendi.", 1, 1, 4, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TrafficFineHistories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TrafficFines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TrafficFines",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TrafficFines",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TrafficFines",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FineTypes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Hız sınırını %10 - %30 aşmak");

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Hız sınırını %30 - %50 aşmak");

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Amount", "ArticleNumber", "Description" },
                values: new object[] { 6439m, "51/2-c", "Hız sınırını %50'den fazla aşmak" });

            migrationBuilder.UpdateData(
                table: "FineTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Amount", "ArticleNumber", "Description" },
                values: new object[] { 690m, "78/1-a", "Emniyet kemeri takmamak" });

            migrationBuilder.InsertData(
                table: "FineTypes",
                columns: new[] { "Id", "Amount", "ArticleNumber", "Description", "IsActive" },
                values: new object[] { 6, 1506m, "73/c", "Seyir halinde cep telefonu kullanmak", true });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Transit");

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Focus");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "ahmet@sirket.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "ayse@sirket.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "FirstName" },
                values: new object[] { "mehmet@sirket.com", "Mehmet" });

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                column: "OwnerName",
                value: "Ahmet Yılmaz");
        }
    }
}
