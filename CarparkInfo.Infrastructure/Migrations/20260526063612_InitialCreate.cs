using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarparkInfo.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarParkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarParkTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarParks",
                columns: table => new
                {
                    CarParkNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    XCoord = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    YCoord = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    CarParkTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParkingSystemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShortTermParking = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FreeParking = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NightParking = table.Column<bool>(type: "INTEGER", nullable: false),
                    CarParkDecks = table.Column<int>(type: "INTEGER", nullable: false),
                    GantryHeight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsBasement = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarParks", x => x.CarParkNo);
                    table.ForeignKey(
                        name: "FK_CarParks_CarParkTypes_CarParkTypeId",
                        column: x => x.CarParkTypeId,
                        principalTable: "CarParkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarParks_ParkingSystems_ParkingSystemId",
                        column: x => x.ParkingSystemId,
                        principalTable: "ParkingSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavourites",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarParkNo = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavourites", x => new { x.UserId, x.CarParkNo });
                    table.ForeignKey(
                        name: "FK_UserFavourites_CarParks_CarParkNo",
                        column: x => x.CarParkNo,
                        principalTable: "CarParks",
                        principalColumn: "CarParkNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavourites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarParks_CarParkTypeId",
                table: "CarParks",
                column: "CarParkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarParks_FreeParking",
                table: "CarParks",
                column: "FreeParking");

            migrationBuilder.CreateIndex(
                name: "IX_CarParks_GantryHeight",
                table: "CarParks",
                column: "GantryHeight");

            migrationBuilder.CreateIndex(
                name: "IX_CarParks_NightParking",
                table: "CarParks",
                column: "NightParking");

            migrationBuilder.CreateIndex(
                name: "IX_CarParks_ParkingSystemId",
                table: "CarParks",
                column: "ParkingSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarParkTypes_Name",
                table: "CarParkTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSystems_Name",
                table: "ParkingSystems",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavourites_CarParkNo",
                table: "UserFavourites",
                column: "CarParkNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavourites");

            migrationBuilder.DropTable(
                name: "CarParks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CarParkTypes");

            migrationBuilder.DropTable(
                name: "ParkingSystems");
        }
    }
}
