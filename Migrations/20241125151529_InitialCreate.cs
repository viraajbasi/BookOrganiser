using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookOrganiser.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoogleBooksID = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", nullable: false),
                    Authors = table.Column<string>(type: "TEXT", nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedDate = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ISBN10 = table.Column<string>(type: "TEXT", nullable: false),
                    ISBN13 = table.Column<string>(type: "TEXT", nullable: false),
                    PageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    GoogleBooksCategories = table.Column<string>(type: "TEXT", nullable: false),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: false),
                    SmallThumbnail = table.Column<string>(type: "TEXT", nullable: false),
                    SmallImage = table.Column<string>(type: "TEXT", nullable: false),
                    MediumImage = table.Column<string>(type: "TEXT", nullable: false),
                    LargeImage = table.Column<string>(type: "TEXT", nullable: false),
                    ExtraLargeImage = table.Column<string>(type: "TEXT", nullable: false),
                    GoogleBooksLink = table.Column<string>(type: "TEXT", nullable: false),
                    CustomCategories = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
