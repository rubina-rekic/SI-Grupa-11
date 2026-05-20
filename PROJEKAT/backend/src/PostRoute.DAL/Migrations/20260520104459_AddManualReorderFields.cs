using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostRoute.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddManualReorderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReorderedAt",
                table: "Routes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastReorderedBy",
                table: "Routes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsManuallyReordered",
                table: "RouteItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReorderedAt",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "LastReorderedBy",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "IsManuallyReordered",
                table: "RouteItems");
        }
    }
}
