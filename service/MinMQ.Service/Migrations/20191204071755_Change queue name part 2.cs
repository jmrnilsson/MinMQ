using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MinMq.Service.Migrations
{
    public partial class Changequeuenamepart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tQueues",
                columns: table => new
                {
                    QueueId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    ByteKey = table.Column<byte>(nullable: false),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tQueues", x => x.QueueId);
                });

            migrationBuilder.CreateTable(
                name: "tMessage",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferenceId = table.Column<long>(nullable: false),
                    NextReferenceId = table.Column<long>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    QueueId = table.Column<int>(nullable: false),
                    MimeTypeId = table.Column<int>(nullable: false),
                    HashCode = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Changed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMessage", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_MimeType",
                        column: x => x.MimeTypeId,
                        principalTable: "tMimeType",
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
                name: "IX_tMessage_MimeTypeId",
                table: "tMessage",
                column: "MimeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tMessage_QueueId",
                table: "tMessage",
                column: "QueueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tMessage");

            migrationBuilder.DropTable(
                name: "tQueues");
        }
    }
}
