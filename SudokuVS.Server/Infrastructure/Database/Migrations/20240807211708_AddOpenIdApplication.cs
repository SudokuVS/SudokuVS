using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudokuVS.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOpenIdApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenIdApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ApplicationType = table.Column<int>(type: "int", nullable: false),
                    ConsentType = table.Column<int>(type: "int", nullable: false),
                    RedirectUris = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIdApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIdApplications_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIdApplications_OwnerId",
                table: "OpenIdApplications",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenIdApplications");
        }
    }
}
