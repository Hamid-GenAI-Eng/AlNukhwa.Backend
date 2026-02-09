using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Misan.Modules.Profiles.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "profiles",
                table: "Profiles",
                columns: new[] { "Id", "AvatarUrl", "City", "CreatedOnUtc", "Dob", "FullName", "Gender", "MembershipTier", "UpdatedOnUtc", "UserId" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999998"), null, "Lahore", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Misan Admin", "Male", "Gold", null, new Guid("99999999-9999-9999-9999-999999999999") });

            migrationBuilder.InsertData(
                schema: "profiles",
                table: "HealthProfiles",
                columns: new[] { "Id", "BloodGroup", "BodyType", "ProfileId" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999997"), "OPositive", "Sanguine", new Guid("99999999-9999-9999-9999-999999999998") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "profiles",
                table: "HealthProfiles",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999997"));

            migrationBuilder.DeleteData(
                schema: "profiles",
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999998"));
        }
    }
}
