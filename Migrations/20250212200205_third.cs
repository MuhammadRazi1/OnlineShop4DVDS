using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop4DVDS.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Albums_AlbumId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GameId",
                table: "Reviews",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Albums_AlbumId",
                table: "Reviews",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "AlbumId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Games_GameId",
                table: "Reviews",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Albums_AlbumId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Games_GameId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_GameId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Albums_AlbumId",
                table: "Reviews",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "AlbumId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
