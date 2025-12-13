using Microsoft.EntityFrameworkCore.Migrations;

namespace AMWE_RealTime_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCredentialStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "PasswordType",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            _ = migrationBuilder.DropColumn(
                name: "PasswordType",
                table: "Users");

            _ = migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");
        }
    }
}