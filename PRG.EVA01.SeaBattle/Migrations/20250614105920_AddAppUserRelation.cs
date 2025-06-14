using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRG.EVA01.SeaBattle.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_UserId",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Games",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_UserId",
                table: "Games",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_UserId",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Games",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_UserId",
                table: "Games",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
