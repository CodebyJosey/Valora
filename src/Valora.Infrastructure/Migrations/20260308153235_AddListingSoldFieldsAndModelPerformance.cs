using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingSoldFieldsAndModelPerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SoldAtUtc",
                table: "Listings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoldPrice",
                table: "Listings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listings_SoldAtUtc",
                table: "Listings",
                column: "SoldAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Listings_SoldAtUtc",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "SoldAtUtc",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "SoldPrice",
                table: "Listings");
        }
    }
}
