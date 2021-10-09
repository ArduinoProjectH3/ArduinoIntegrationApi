using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArduinoIntegrationApi.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurtainReading",
                columns: table => new
                {
                    Cr_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cr_Value = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurtainReading", x => x.Cr_Id);
                });

            migrationBuilder.CreateTable(
                name: "HumidityReading",
                columns: table => new
                {
                    Hr_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hr_Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HumidityReading", x => x.Hr_Id);
                });

            migrationBuilder.CreateTable(
                name: "LightReading",
                columns: table => new
                {
                    Lr_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lr_Value = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightReading", x => x.Lr_Id);
                });

            migrationBuilder.CreateTable(
                name: "SoundReading",
                columns: table => new
                {
                    Sr_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sr_Value = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoundReading", x => x.Sr_Id);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureReading",
                columns: table => new
                {
                    Tr_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tr_Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureReading", x => x.Tr_Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomReading",
                columns: table => new
                {
                    Rr_RoomName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Rr_Cts = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lr_Id = table.Column<int>(type: "int", nullable: true),
                    Tr_HeadTr_Id = table.Column<int>(type: "int", nullable: true),
                    Tr_FeetTr_Id = table.Column<int>(type: "int", nullable: true),
                    Hr_Id = table.Column<int>(type: "int", nullable: true),
                    Cr_Id = table.Column<int>(type: "int", nullable: true),
                    Sr_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomReading", x => new { x.Rr_RoomName, x.Rr_Cts });
                    table.ForeignKey(
                        name: "FK_RoomReading_CurtainReading_Cr_Id",
                        column: x => x.Cr_Id,
                        principalTable: "CurtainReading",
                        principalColumn: "Cr_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomReading_HumidityReading_Hr_Id",
                        column: x => x.Hr_Id,
                        principalTable: "HumidityReading",
                        principalColumn: "Hr_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomReading_LightReading_Lr_Id",
                        column: x => x.Lr_Id,
                        principalTable: "LightReading",
                        principalColumn: "Lr_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomReading_SoundReading_Sr_Id",
                        column: x => x.Sr_Id,
                        principalTable: "SoundReading",
                        principalColumn: "Sr_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomReading_TemperatureReading_Tr_FeetTr_Id",
                        column: x => x.Tr_FeetTr_Id,
                        principalTable: "TemperatureReading",
                        principalColumn: "Tr_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomReading_TemperatureReading_Tr_HeadTr_Id",
                        column: x => x.Tr_HeadTr_Id,
                        principalTable: "TemperatureReading",
                        principalColumn: "Tr_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomReading_Cr_Id",
                table: "RoomReading",
                column: "Cr_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReading_Hr_Id",
                table: "RoomReading",
                column: "Hr_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReading_Lr_Id",
                table: "RoomReading",
                column: "Lr_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReading_Sr_Id",
                table: "RoomReading",
                column: "Sr_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReading_Tr_FeetTr_Id",
                table: "RoomReading",
                column: "Tr_FeetTr_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReading_Tr_HeadTr_Id",
                table: "RoomReading",
                column: "Tr_HeadTr_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomReading");

            migrationBuilder.DropTable(
                name: "CurtainReading");

            migrationBuilder.DropTable(
                name: "HumidityReading");

            migrationBuilder.DropTable(
                name: "LightReading");

            migrationBuilder.DropTable(
                name: "SoundReading");

            migrationBuilder.DropTable(
                name: "TemperatureReading");
        }
    }
}
