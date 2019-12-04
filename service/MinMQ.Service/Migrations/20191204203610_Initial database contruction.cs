using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    public partial class Initialdatabasecontruction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "tMimeTypes",
                columns: table => new
                {
                    MimeTypeId = table.Column<short>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Expression = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMimeTypes", x => x.MimeTypeId);
                });

            migrationBuilder.CreateTable(
                name: "tQueues",
                columns: table => new
                {
                    QueueId = table.Column<short>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tQueues", x => x.QueueId);
                });

            migrationBuilder.CreateTable(
                name: "tMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferenceId = table.Column<long>(nullable: false),
                    NextReferenceId = table.Column<long>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    QueueId = table.Column<short>(nullable: false),
                    MimeTypeId = table.Column<short>(nullable: false),
                    HashCode = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_MimeType",
                        column: x => x.MimeTypeId,
                        principalTable: "tMimeTypes",
                        principalColumn: "MimeTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_Queue",
                        column: x => x.QueueId,
                        principalTable: "tQueues",
                        principalColumn: "QueueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tMessages_MimeTypeId",
                table: "tMessages",
                column: "MimeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tMessages_QueueId",
                table: "tMessages",
                column: "QueueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tCursors");

            migrationBuilder.DropTable(
                name: "tMessages");

            migrationBuilder.DropTable(
                name: "tMimeTypes");

            migrationBuilder.DropTable(
                name: "tQueues");
        }
    }
}
