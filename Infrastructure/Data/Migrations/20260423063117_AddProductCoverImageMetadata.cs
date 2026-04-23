using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Catalog.API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCoverImageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductCoverImages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ProductCoverImages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "ProductCoverImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductCoverImages");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ProductCoverImages");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "ProductCoverImages");
        }
    }
}
