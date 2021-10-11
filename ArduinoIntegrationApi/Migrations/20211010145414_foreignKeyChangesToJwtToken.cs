using Microsoft.EntityFrameworkCore.Migrations;

namespace ArduinoIntegrationApi.Migrations
{
    public partial class foreignKeyChangesToJwtToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JwtTokens_Users_Username",
                table: "JwtTokens");

            migrationBuilder.DropIndex(
                name: "IX_JwtTokens_Username",
                table: "JwtTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JwtTokens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JwtTokens",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JwtTokens_Username",
                table: "JwtTokens",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_JwtTokens_Users_Username",
                table: "JwtTokens",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
