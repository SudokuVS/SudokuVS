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
            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_UserName",
                table: "PlayerStates",
                column: "UserName",
                unique: true);
        }
    }
}
