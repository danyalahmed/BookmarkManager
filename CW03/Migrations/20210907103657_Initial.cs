using Microsoft.EntityFrameworkCore.Migrations;

namespace CW03.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookmarkEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookmarkType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ReadOnly = table.Column<bool>(nullable: false),
                    ParentPath = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookmarkEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookmarkEntity");
        }
    }
}
