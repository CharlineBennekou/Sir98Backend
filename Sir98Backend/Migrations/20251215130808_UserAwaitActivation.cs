using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sir98Backend.Migrations
{
    /// <inheritdoc />
    public partial class UserAwaitActivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersAwaitingActivation",
                columns: table => new
                {
                    ActivationCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersAwaitingActivation", x => x.ActivationCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersAwaitingActivation_Email",
                table: "UsersAwaitingActivation",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersAwaitingActivation");
        }
    }
}
