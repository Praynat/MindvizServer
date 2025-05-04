using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindvizServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssignedUserIdsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make AssignedUserIds nullable
            migrationBuilder.AlterColumn<string>(
                name: "AssignedUserIds",
                table: "GroupTasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.Sql("UPDATE GroupTasks SET AssignedUserIds = '[]' WHERE AssignedUserIds IS NULL OR AssignedUserIds = ''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert back to non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "AssignedUserIds",
                table: "GroupTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

    }
}
