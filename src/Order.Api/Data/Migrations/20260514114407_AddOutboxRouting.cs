using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Exchange",
                table: "OutboxMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoutingKey",
                table: "OutboxMessages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exchange",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RoutingKey",
                table: "OutboxMessages");
        }
    }
}
