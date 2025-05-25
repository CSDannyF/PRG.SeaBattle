using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRG.EVA01.SeaBattle.Migrations
{
    /// <inheritdoc />
    public partial class GameGamelogRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GameLogs_GameId",
                table: "GameLogs",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameLogs_Games_GameId",
                table: "GameLogs",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameLogs_Games_GameId",
                table: "GameLogs");

            migrationBuilder.DropIndex(
                name: "IX_GameLogs_GameId",
                table: "GameLogs");
        }
    }
}
