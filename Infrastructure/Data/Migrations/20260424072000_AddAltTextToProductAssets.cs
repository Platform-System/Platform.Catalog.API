using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Catalog.API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAltTextToProductAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "ProductCoverImages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AltText",
                table: "ProductMedias",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltText",
                table: "ProductCoverImages");

            migrationBuilder.AlterColumn<string>(
                name: "AltText",
                table: "ProductMedias",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
