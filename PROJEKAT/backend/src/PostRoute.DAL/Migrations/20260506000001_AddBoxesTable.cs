using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostRoute.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBoxesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    YearOfInstallation = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_SerialNumber",
                table: "Boxes",
                column: "SerialNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boxes");
        }
    }
}
