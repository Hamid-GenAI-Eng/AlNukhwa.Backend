using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Misan.Modules.Store.Migrations
{
    /// <inheritdoc />
    public partial class InitialStoreV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "store",
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Mizaj", "Name", "Price", "Rating", "ReviewCount", "SalePrice", "StockQty" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "Food", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pure Honey", "http://example.com/honey.png", true, 2, "Seed Honey", 50m, 0.0, 0, null, 100 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "store",
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
