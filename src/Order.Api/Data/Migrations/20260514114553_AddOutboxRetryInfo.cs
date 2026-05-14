using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxRetryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "OutboxMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "OutboxMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "OutboxMessages");
        }
    }
}
