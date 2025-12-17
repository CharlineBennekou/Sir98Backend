using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sir98Backend.Migrations
{
    /// <inheritdoc />
    public partial class PushSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PushSubscriptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "PushSubscriptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "PushSubscriptions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUsedUtc",
                table: "PushSubscriptions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "AllOccurrences",
                table: "ActivitySubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_Endpoint",
                table: "PushSubscriptions",
                column: "Endpoint",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySubscriptions_ActivityId_OriginalStartUtc_AllOccurrences",
                table: "ActivitySubscriptions",
                columns: new[] { "ActivityId", "OriginalStartUtc", "AllOccurrences" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_Endpoint",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ActivitySubscriptions_ActivityId_OriginalStartUtc_AllOccurrences",
                table: "ActivitySubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "LastUsedUtc",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "AllOccurrences",
                table: "ActivitySubscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PushSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "PushSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
