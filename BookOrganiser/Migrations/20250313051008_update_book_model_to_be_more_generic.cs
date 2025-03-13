using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookOrganiser.Migrations
{
    /// <inheritdoc />
    public partial class update_book_model_to_be_more_generic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoogleBooksLink",
                table: "Books",
                newName: "UpstreamLink");

            migrationBuilder.RenameColumn(
                name: "GoogleBooksID",
                table: "Books",
                newName: "UpstreamId");

            migrationBuilder.RenameColumn(
                name: "GoogleBooksCategories",
                table: "Books",
                newName: "UpstreamCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpstreamLink",
                table: "Books",
                newName: "GoogleBooksLink");

            migrationBuilder.RenameColumn(
                name: "UpstreamId",
                table: "Books",
                newName: "GoogleBooksID");

            migrationBuilder.RenameColumn(
                name: "UpstreamCategories",
                table: "Books",
                newName: "GoogleBooksCategories");
        }
    }
}
