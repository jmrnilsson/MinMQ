using Microsoft.EntityFrameworkCore.Migrations;

namespace MinMq.Service.Migrations
{
    public partial class AddbytelookupkeytoQueueaswell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ByteKey",
                table: "Queues",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<long>(
                name: "NextReferenceId",
                table: "Messages",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByteKey",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "NextReferenceId",
                table: "Messages");
        }
    }
}
