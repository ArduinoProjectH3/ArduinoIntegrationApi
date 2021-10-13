using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArduinoIntegrationApi.Migrations
{
    public partial class changedJwtTokensTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JwtTokens",
                table: "JwtTokens");

            migrationBuilder.DropColumn(
                name: "JwtToken_Id",
                table: "JwtTokens");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "JwtTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JwtTokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JwtTokens",
                table: "JwtTokens",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JwtTokens",
                table: "JwtTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JwtTokens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "JwtToken_Id",
                table: "JwtTokens",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "JwtTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_JwtTokens",
                table: "JwtTokens",
                column: "JwtToken_Id");
        }
    }
}
