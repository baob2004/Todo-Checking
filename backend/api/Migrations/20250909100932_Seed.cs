using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TodoItems",
                columns: new[] { "Id", "Due", "IsDone", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Learn EF Core" },
                    { 2, new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Setup Swagger UI" },
                    { 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Write API docs" },
                    { 4, new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Finish Clean Architecture tutorial" },
                    { 5, new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Implement JWT Auth" },
                    { 6, new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Create Todo CRUD" },
                    { 7, new DateTime(2025, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Write Unit Tests" },
                    { 8, new DateTime(2025, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Configure CORS" },
                    { 9, new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Add Logging with NLog" },
                    { 10, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Deploy API to Azure" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
