using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindvizServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRootToTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRoot",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRoot",
                table: "Tasks");
        }
    }
}
