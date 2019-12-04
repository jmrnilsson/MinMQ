using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    public partial class Changequeuenametemporarily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Queues");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Queues",
                columns: table => new
                {
                    QueueId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Added = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ByteKey = table.Column<byte>(type: "smallint", nullable: false),
                    Changed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.QueueId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Added = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Changed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    HashCode = table.Column<string>(type: "text", nullable: true),
                    MimeTypeId = table.Column<int>(type: "integer", nullable: false),
                    NextReferenceId = table.Column<long>(type: "bigint", nullable: false),
                    QueueId = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_MimeType",
                        column: x => x.MimeTypeId,
                        principalTable: "tMimeType",
                        principalColumn: "MimeTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_Queue",
                        column: x => x.QueueId,
                        principalTable: "Queues",
                        principalColumn: "QueueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MimeTypeId",
                table: "Messages",
                column: "MimeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_QueueId",
                table: "Messages",
                column: "QueueId");
        }
    }
}
