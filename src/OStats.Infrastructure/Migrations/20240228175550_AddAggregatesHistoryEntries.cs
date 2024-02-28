using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OStats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAggregatesHistoryEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregatesHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateType = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    EventData = table.Column<string>(type: "text", nullable: false),
                    EventDescription = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatesHistoryEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatesHistoryEntries_AggregateId",
                table: "AggregatesHistoryEntries",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatesHistoryEntries_UserId",
                table: "AggregatesHistoryEntries",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatesHistoryEntries");
        }
    }
}
