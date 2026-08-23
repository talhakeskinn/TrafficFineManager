using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace trafficFineManager.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "TrafficFines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "TrafficFines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "İstanbul" },
                    { 2, "Ankara" }
                });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "Id", "CityId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Kadıköy" },
                    { 2, 1, "Beşiktaş" },
                    { 3, 1, "Şişli" },
                    { 4, 2, "Çankaya" },
                    { 5, 2, "Yenimahalle" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_CityId",
                table: "TrafficFines",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_DistrictId",
                table: "TrafficFines",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId",
                table: "Districts",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficFines_Cities_CityId",
                table: "TrafficFines",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficFines_Districts_DistrictId",
                table: "TrafficFines",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrafficFines_Cities_CityId",
                table: "TrafficFines");

            migrationBuilder.DropForeignKey(
                name: "FK_TrafficFines_Districts_DistrictId",
                table: "TrafficFines");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_TrafficFines_CityId",
                table: "TrafficFines");

            migrationBuilder.DropIndex(
                name: "IX_TrafficFines_DistrictId",
                table: "TrafficFines");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "TrafficFines");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "TrafficFines");
        }
    }
}
