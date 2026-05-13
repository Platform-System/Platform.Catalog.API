using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Catalog.API.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumnsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "ProductMedias",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "ProductCoverImages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "ProductMedias");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "ProductCoverImages");
        }
    }
}
