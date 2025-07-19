using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allinone.DLL.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeCascadeCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiaryDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaryID = table.Column<int>(type: "int", nullable: false),
                    DiaryTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DiaryDetail_DiaryType_DiaryTypeID",
                        column: x => x.DiaryTypeID,
                        principalTable: "DiaryType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiaryDetail_Diary_DiaryID",
                        column: x => x.DiaryID,
                        principalTable: "Diary",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryDetail_DiaryID",
                table: "DiaryDetail",
                column: "DiaryID");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryDetail_DiaryTypeID",
                table: "DiaryDetail",
                column: "DiaryTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaryDetail");
        }
    }
}
