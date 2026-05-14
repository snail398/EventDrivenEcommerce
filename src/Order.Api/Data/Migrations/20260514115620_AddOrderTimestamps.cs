using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOnUtc",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedOnUtc",
                table: "Orders");
        }
    }
}
