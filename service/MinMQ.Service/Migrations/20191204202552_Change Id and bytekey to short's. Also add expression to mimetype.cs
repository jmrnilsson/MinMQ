using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    public partial class ChangeIdandbytekeytoshortsAlsoaddexpressiontomimetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tMimeType",
                table: "tMimeType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tMessage",
                table: "tMessage");

            migrationBuilder.DropColumn(
                name: "ByteKey",
                table: "tQueues");

            migrationBuilder.DropColumn(
                name: "ByteKey",
                table: "tMimeType");

            migrationBuilder.RenameTable(
                name: "tMimeType",
                newName: "tMimeTypes");

            migrationBuilder.RenameTable(
                name: "tMessage",
                newName: "tMessages");

            migrationBuilder.RenameIndex(
                name: "IX_tMessage_QueueId",
                table: "tMessages",
                newName: "IX_tMessages_QueueId");

            migrationBuilder.RenameIndex(
                name: "IX_tMessage_MimeTypeId",
                table: "tMessages",
                newName: "IX_tMessages_MimeTypeId");

            migrationBuilder.AlterColumn<short>(
                name: "QueueId",
                table: "tQueues",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<short>(
                name: "MimeTypeId",
                table: "tMimeTypes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Expression",
                table: "tMimeTypes",
                nullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "QueueId",
                table: "tMessages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<short>(
                name: "MimeTypeId",
                table: "tMessages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tMimeTypes",
                table: "tMimeTypes",
                column: "MimeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tMessages",
                table: "tMessages",
                column: "MessageId");

            migrationBuilder.CreateTable(
                name: "tCursors",
                columns: table => new
                {
                    CursorId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false),
                    NextReferenceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCursors", x => x.CursorId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tCursors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tMimeTypes",
                table: "tMimeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tMessages",
                table: "tMessages");

            migrationBuilder.DropColumn(
                name: "Expression",
                table: "tMimeTypes");

            migrationBuilder.RenameTable(
                name: "tMimeTypes",
                newName: "tMimeType");

            migrationBuilder.RenameTable(
                name: "tMessages",
                newName: "tMessage");

            migrationBuilder.RenameIndex(
                name: "IX_tMessages_QueueId",
                table: "tMessage",
                newName: "IX_tMessage_QueueId");

            migrationBuilder.RenameIndex(
                name: "IX_tMessages_MimeTypeId",
                table: "tMessage",
                newName: "IX_tMessage_MimeTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "QueueId",
                table: "tQueues",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<short>(
                name: "ByteKey",
                table: "tQueues",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "MimeTypeId",
                table: "tMimeType",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<short>(
                name: "ByteKey",
                table: "tMimeType",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "QueueId",
                table: "tMessage",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<int>(
                name: "MimeTypeId",
                table: "tMessage",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tMimeType",
                table: "tMimeType",
                column: "MimeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tMessage",
                table: "tMessage",
                column: "MessageId");
        }
    }
}
