using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostRoute.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMailboxAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlwaysAvailable",
                table: "Mailboxes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Slot1End",
                table: "Mailboxes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Slot1Start",
                table: "Mailboxes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Slot2End",
                table: "Mailboxes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Slot2Start",
                table: "Mailboxes",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlwaysAvailable",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Slot1End",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Slot1Start",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Slot2End",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Slot2Start",
                table: "Mailboxes");
        }
    }
}
