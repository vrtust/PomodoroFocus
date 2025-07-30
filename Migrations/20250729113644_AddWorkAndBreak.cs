using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PomodoroFocus.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkAndBreak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Sessions",
                newName: "PlannedWorkDurationMinutes");

            migrationBuilder.AddColumn<int>(
                name: "ActualBreakDurationMinutes",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActualWorkDurationMinutes",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakDurationMinutes",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualBreakDurationMinutes",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ActualWorkDurationMinutes",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "PlannedBreakDurationMinutes",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "PlannedWorkDurationMinutes",
                table: "Sessions",
                newName: "DurationMinutes");
        }
    }
}
