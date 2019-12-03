using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    public partial class Old_school_table_names_Add_FK_MimeType_Cursor_Added_Changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MimeType");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Messages");

            migrationBuilder.AddColumn<DateTime>(
                name: "Added",
                table: "Queues",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Changed",
                table: "Queues",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Added",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Changed",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MimeTypeId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tMimeType",
                columns: table => new
                {
                    MimeTypeId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ByteKey = table.Column<byte>(nullable: false),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMimeType", x => x.MimeTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MimeTypeId",
                table: "Messages",
                column: "MimeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_MimeType",
                table: "Messages",
                column: "MimeTypeId",
                principalTable: "tMimeType",
                principalColumn: "MimeTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_MimeType",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "tMimeType");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MimeTypeId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Added",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "Changed",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "Added",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Changed",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MimeTypeId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MimeType",
                columns: table => new
                {
                    MimeTypeId = table.Column<string>(type: "text", nullable: false),
                    ByteKey = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MimeType", x => x.MimeTypeId);
                });
        }
    }
}
