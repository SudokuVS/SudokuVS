using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuVS.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNavigatonToPlayerState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[PlayerStates]");
            migrationBuilder.Sql("DELETE FROM [dbo].[Games]");
            
            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "PlayerStates");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PlayerStates",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_UserId",
                table: "PlayerStates",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_AspNetUsers_UserId",
                table: "PlayerStates",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_AspNetUsers_UserId",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_UserId",
                table: "PlayerStates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlayerStates");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "PlayerStates",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates",
                column: "UserName");
        }
    }
}
