using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostRoute.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteAssignmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Routes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                table: "Routes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "Routes");
        }
    }
}
