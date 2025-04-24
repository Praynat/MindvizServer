using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindvizServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCheckedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First add the IsChecked column
            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Tasks",
                nullable: false,
                defaultValue: false);

            // Then create a SQL statement to update existing tasks
            // Set IsChecked to true where Progress is 100
            migrationBuilder.Sql(@"
                UPDATE Tasks
                SET IsChecked = 1
                WHERE Progress = 100;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Tasks");
        }
    }
}
