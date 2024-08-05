using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuVS.Game.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SaveUsernameInstadOfUserIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_Users_UserId",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_UserId",
                table: "PlayerStates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlayerStates");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "PlayerStates",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "PlayerStates");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PlayerStates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_UserId",
                table: "PlayerStates",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_Users_UserId",
                table: "PlayerStates",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
