using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace AMWE_RealTime_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddReportHubState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "ReportHubState",
                columns: table => new
                {
                    Id = table.Column<bool>(type: "boolean", nullable: false),
                    WorkdayValue = table.Column<bool>(type: "boolean", nullable: false),
                    BaseRepInterval = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_ReportHubState", x => x.Id));

            _ = migrationBuilder.InsertData(
                table: "ReportHubState",
                columns: new[] { "Id", "BaseRepInterval", "WorkdayValue" },
                values: new object[] { true, new TimeSpan(0, 0, 1, 0, 0), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "ReportHubState");
        }
    }
}