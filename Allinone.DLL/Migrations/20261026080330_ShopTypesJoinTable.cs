using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allinone.DLL.Migrations
{
    /// <inheritdoc />
    public partial class ShopTypesJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopShopType",
                columns: table => new
                {
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    ShopTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopShopType", x => new { x.ShopID, x.ShopTypeID });
                    table.ForeignKey(
                        name: "FK_ShopShopType_ShopType_ShopTypeID",
                        column: x => x.ShopTypeID,
                        principalTable: "ShopType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopShopType_Shop_ShopID",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopShopType_ShopTypeID",
                table: "ShopShopType",
                column: "ShopTypeID");

            // Migrate existing comma-separated Types into the join table (valid, same-member types only)
            migrationBuilder.Sql(@"
INSERT INTO ShopShopType (ShopID, ShopTypeID)
SELECT DISTINCT s.ID, st.ID
FROM Shop s
CROSS APPLY STRING_SPLIT(s.Types, ',') v
JOIN ShopType st ON st.ID = TRY_CAST(LTRIM(RTRIM(v.value)) AS int) AND st.MemberID = s.MemberID
WHERE s.Types IS NOT NULL AND s.Types <> '';");

            migrationBuilder.DropColumn(
                name: "Types",
                table: "Shop");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Types",
                table: "Shop",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
UPDATE s SET Types = ISNULL((
    SELECT STRING_AGG(CAST(j.ShopTypeID AS nvarchar(20)), ',') FROM ShopShopType j WHERE j.ShopID = s.ID), '')
FROM Shop s;");

            migrationBuilder.DropTable(
                name: "ShopShopType");
        }
    }
}
