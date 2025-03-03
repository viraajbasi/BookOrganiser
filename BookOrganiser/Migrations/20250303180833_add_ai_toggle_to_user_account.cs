using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookOrganiser.Migrations
{
    /// <inheritdoc />
    public partial class add_ai_toggle_to_user_account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptedAIFeatures",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAIFeatures",
                table: "AspNetUsers");
        }
    }
}
