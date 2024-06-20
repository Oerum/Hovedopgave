using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Database.Migrations
{
    /// <inheritdoc />
    public partial class _04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_DiscordUsername",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_DiscordUsername",
                table: "User",
                column: "DiscordUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_DiscordUsername",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_DiscordUsername",
                table: "User",
                column: "DiscordUsername",
                unique: true);
        }
    }
}
