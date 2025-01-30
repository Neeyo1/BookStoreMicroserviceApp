using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNamePropertyToBookEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Books",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Books");
        }
    }
}
