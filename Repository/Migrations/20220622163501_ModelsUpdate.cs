using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class ModelsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Tickets_TicketId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_TicketId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Genres");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleasedDate",
                table: "Tickets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Tickets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleasedDate",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Tickets");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "TicketId",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_TicketId",
                table: "Genres",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Tickets_TicketId",
                table: "Genres",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
