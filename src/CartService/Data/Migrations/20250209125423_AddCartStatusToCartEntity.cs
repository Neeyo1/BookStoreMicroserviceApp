using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCartStatusToCartEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "Carts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Carts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Carts");
        }
    }
}
