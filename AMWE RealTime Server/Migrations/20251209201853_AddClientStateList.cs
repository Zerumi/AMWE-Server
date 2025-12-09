using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AMWE_RealTime_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddClientStateList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalClientStatesList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<long>(type: "bigint", nullable: true),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnhanced = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogoutDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalClientStatesList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlobalClientStatesList_GlobalClientsList_ClientId",
                        column: x => x.ClientId,
                        principalTable: "GlobalClientsList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalClientStatesList_ClientId",
                table: "GlobalClientStatesList",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalClientStatesList");
        }
    }
}
