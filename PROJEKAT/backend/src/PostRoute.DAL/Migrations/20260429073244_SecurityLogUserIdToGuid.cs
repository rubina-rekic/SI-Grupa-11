using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostRoute.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SecurityLogUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""SecurityLogs""
                  ALTER COLUMN ""UserId"" TYPE uuid
                  USING NULLIF(""UserId"", '')::uuid;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SecurityLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
