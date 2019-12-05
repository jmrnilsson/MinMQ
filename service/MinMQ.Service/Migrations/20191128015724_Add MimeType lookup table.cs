using Microsoft.EntityFrameworkCore.Migrations;

namespace MinMq.Service.Migrations
{
    public partial class AddMimeTypelookuptable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MimeType",
                columns: table => new
                {
                    MimeTypeId = table.Column<string>(nullable: false),
                    ByteKey = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MimeType", x => x.MimeTypeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MimeType");
        }
    }
}
