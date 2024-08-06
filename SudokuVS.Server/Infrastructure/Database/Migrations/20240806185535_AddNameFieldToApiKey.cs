using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuVS.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNameFieldToApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ApiKeys",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ApiKeys");
        }
    }
}
