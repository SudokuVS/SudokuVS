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
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "PlayerStates",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "PlayerStates",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_AspNetUsers_UserName",
                table: "PlayerStates",
                column: "UserName",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_AspNetUsers_UserName",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "PlayerStates",
                newName: "Username");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "PlayerStates",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
