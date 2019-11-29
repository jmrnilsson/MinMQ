using Microsoft.EntityFrameworkCore.Migrations;

namespace MinMq.Service.Migrations
{
    public partial class AddHashCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HashCode",
                table: "Messages",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashCode",
                table: "Messages");
        }
    }
}
