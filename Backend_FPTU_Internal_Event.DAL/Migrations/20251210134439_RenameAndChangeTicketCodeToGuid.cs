using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_FPTU_Internal_Event.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameAndChangeTicketCodeToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TichketCode",
                table: "Tickets");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketCode",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketCode",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "TichketCode",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
