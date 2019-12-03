﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MinMq.Service.Migrations
{
#pragma warning disable SA1300 // Element should begin with upper-case letter
	public partial class add : Migration
#pragma warning restore SA1300 // Element should begin with upper-case letter
	{
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Queues_QueueId",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "QueueId",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Queue",
                table: "Messages",
                column: "QueueId",
                principalTable: "Queues",
                principalColumn: "QueueId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Queue",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "QueueId",
                table: "Messages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Queues_QueueId",
                table: "Messages",
                column: "QueueId",
                principalTable: "Queues",
                principalColumn: "QueueId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
