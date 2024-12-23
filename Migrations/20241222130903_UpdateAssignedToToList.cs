﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindvizServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssignedToToList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
