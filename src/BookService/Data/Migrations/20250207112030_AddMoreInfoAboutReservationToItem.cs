using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreInfoAboutReservationToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReservedBy",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedUntil",
                table: "Items",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedBy",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ReservedUntil",
                table: "Items");
        }
    }
}
