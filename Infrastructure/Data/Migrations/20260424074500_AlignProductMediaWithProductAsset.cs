using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Catalog.API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignProductMediaWithProductAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                table: "ProductMedias",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ContainerName",
                table: "ProductMedias",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "ProductMedias",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductMedias",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobName",
                table: "ProductMedias");

            migrationBuilder.DropColumn(
                name: "ContainerName",
                table: "ProductMedias");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "ProductMedias",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductMedias",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
