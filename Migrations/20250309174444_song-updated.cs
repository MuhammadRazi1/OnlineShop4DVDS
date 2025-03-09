using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop4DVDS.Migrations
{
    /// <inheritdoc />
    public partial class songupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistAge",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "ArtistBio",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "ArtistDateOfBirth",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "ArtistImage",
                table: "Artists");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Songs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Songs");

            migrationBuilder.AddColumn<int>(
                name: "ArtistAge",
                table: "Artists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ArtistBio",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArtistDateOfBirth",
                table: "Artists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ArtistImage",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
