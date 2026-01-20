using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sir98Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedActivityIndexFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_ChangedActivities_ActivityId", table: "ChangedActivities");
            migrationBuilder.CreateIndex(name: "IX_ChangedActivities_ActivityId_OriginalStartUtc", table: "ChangedActivities", columns: new[] { "ActivityId", "OriginalStartUtc" }, unique: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_ChangedActivities_ActivityId_OriginalStartUtc", table: "ChangedActivities");
            migrationBuilder.CreateIndex(name: "IX_ChangedActivities_ActivityId", table: "ChangedActivities", column: "ActivityId", unique: true);
        }
    }
}
