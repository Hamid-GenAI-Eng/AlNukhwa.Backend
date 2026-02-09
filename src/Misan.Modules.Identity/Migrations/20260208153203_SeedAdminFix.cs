using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Misan.Modules.Identity.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "Users",
                columns: new[] { "Id", "CreatedOnUtc", "Email", "IsEmailVerified", "IsPhoneVerified", "PasswordHash", "Phone", "Role", "UpdatedOnUtc" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MisanAdmin@codeenvision.com", true, true, "$2a$11$wnzGoYEsJMQfVA5Fqn2GqOgA4DfgZ.hqCQx3jfnu5oTgnfgYIOxta", "+920000000000", "Admin", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));
        }
    }
}
