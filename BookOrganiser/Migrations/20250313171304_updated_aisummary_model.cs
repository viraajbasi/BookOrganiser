using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookOrganiser.Migrations
{
    /// <inheritdoc />
    public partial class updated_aisummary_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedAt",
                table: "AISummary",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "AISummary",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedAt",
                table: "AISummary");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "AISummary");
        }
    }
}
