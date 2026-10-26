using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allinone.DLL.Migrations
{
    /// <inheritdoc />
    public partial class AddTodolistDoneIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodolistDone_TodolistID",
                table: "TodolistDone");

            migrationBuilder.CreateIndex(
                name: "IX_TodolistDone_TodolistID_UpdateDate",
                table: "TodolistDone",
                columns: new[] { "TodolistID", "UpdateDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodolistDone_TodolistID_UpdateDate",
                table: "TodolistDone");

            migrationBuilder.CreateIndex(
                name: "IX_TodolistDone_TodolistID",
                table: "TodolistDone",
                column: "TodolistID");
        }
    }
}
