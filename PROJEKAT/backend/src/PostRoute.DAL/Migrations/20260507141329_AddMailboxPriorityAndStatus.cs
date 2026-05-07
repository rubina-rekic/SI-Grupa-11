using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostRoute.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMailboxPriorityAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Mailboxes",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Mailboxes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"UPDATE ""Mailboxes"" SET ""Priority"" = 1 WHERE ""Type"" = 4;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Mailboxes");
        }
    }
}
