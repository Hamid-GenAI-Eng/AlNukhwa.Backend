using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Misan.Modules.Practitioner.Migrations
{
    /// <inheritdoc />
    public partial class InitialPractitioner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "practitioner");

            migrationBuilder.CreateTable(
                name: "Clinics",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    MapLat = table.Column<double>(type: "double precision", nullable: false),
                    MapLng = table.Column<double>(type: "double precision", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hakeems",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CnicNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    VerificationStatus = table.Column<string>(type: "text", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hakeems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleBreaks",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleBreaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleConfigs",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    BufferTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    NoticePeriodHours = table.Column<int>(type: "integer", nullable: false),
                    AutoAccept = table.Column<bool>(type: "boolean", nullable: false),
                    Timezone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClinicFees",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicFees_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalSchema: "practitioner",
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicSchedules",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    MorningStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    MorningEnd = table.Column<TimeSpan>(type: "interval", nullable: false),
                    AfternoonStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    AfternoonEnd = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicSchedules_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalSchema: "practitioner",
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicServices",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicServices_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalSchema: "practitioner",
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HakeemDocuments",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakeemDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HakeemDocuments_Hakeems_HakeemId",
                        column: x => x.HakeemId,
                        principalSchema: "practitioner",
                        principalTable: "Hakeems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HakeemLanguages",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakeemLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HakeemLanguages_Hakeems_HakeemId",
                        column: x => x.HakeemId,
                        principalSchema: "practitioner",
                        principalTable: "Hakeems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HakeemQualifications",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakeemQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HakeemQualifications_Hakeems_HakeemId",
                        column: x => x.HakeemId,
                        principalSchema: "practitioner",
                        principalTable: "Hakeems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HakeemSpecializations",
                schema: "practitioner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HakeemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakeemSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HakeemSpecializations_Hakeems_HakeemId",
                        column: x => x.HakeemId,
                        principalSchema: "practitioner",
                        principalTable: "Hakeems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicFees_ClinicId",
                schema: "practitioner",
                table: "ClinicFees",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_HakeemId",
                schema: "practitioner",
                table: "Clinics",
                column: "HakeemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicSchedules_ClinicId",
                schema: "practitioner",
                table: "ClinicSchedules",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicServices_ClinicId",
                schema: "practitioner",
                table: "ClinicServices",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_HakeemDocuments_HakeemId",
                schema: "practitioner",
                table: "HakeemDocuments",
                column: "HakeemId");

            migrationBuilder.CreateIndex(
                name: "IX_HakeemLanguages_HakeemId",
                schema: "practitioner",
                table: "HakeemLanguages",
                column: "HakeemId");

            migrationBuilder.CreateIndex(
                name: "IX_HakeemQualifications_HakeemId",
                schema: "practitioner",
                table: "HakeemQualifications",
                column: "HakeemId");

            migrationBuilder.CreateIndex(
                name: "IX_Hakeems_UserId",
                schema: "practitioner",
                table: "Hakeems",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HakeemSpecializations_HakeemId",
                schema: "practitioner",
                table: "HakeemSpecializations",
                column: "HakeemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleConfigs_HakeemId",
                schema: "practitioner",
                table: "ScheduleConfigs",
                column: "HakeemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicFees",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "ClinicSchedules",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "ClinicServices",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "HakeemDocuments",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "HakeemLanguages",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "HakeemQualifications",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "HakeemSpecializations",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "ScheduleBreaks",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "ScheduleConfigs",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "Clinics",
                schema: "practitioner");

            migrationBuilder.DropTable(
                name: "Hakeems",
                schema: "practitioner");
        }
    }
}
