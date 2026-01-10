using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allinone.DLL.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberRoleAndIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Member",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Member",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Member");
        }
    }
}
