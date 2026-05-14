using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql("""
                ALTER TABLE "Orders"
                ALTER COLUMN "Status" TYPE integer
                USING CASE "Status"
                    WHEN 'Created' THEN 1
                    WHEN 'Confirmed' THEN 2
                    WHEN 'Failed' THEN 3
                    ELSE 1
                END;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Orders"
                ALTER COLUMN "Status" TYPE text
                USING CASE "Status"
                    WHEN 1 THEN 'Created'
                    WHEN 2 THEN 'Confirmed'
                    WHEN 3 THEN 'Failed'
                    ELSE 'Created'
                END;
            """);
        }
    }
}
