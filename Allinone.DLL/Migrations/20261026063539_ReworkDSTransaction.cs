using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allinone.DLL.Migrations
{
    /// <inheritdoc />
    public partial class ReworkDSTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_MemberID",
                table: "DSTransaction");

            migrationBuilder.RenameColumn(
                name: "DSTransferOutID",
                table: "DSTransaction",
                newName: "TransferOutTransactionID");

            migrationBuilder.AlterColumn<int>(
                name: "TransferOutTransactionID",
                table: "DSTransaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DSItemSubID",
                table: "DSTransaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DSItemID",
                table: "DSTransaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // 0 meant "none"; convert to NULL (and drop dangling references) so the new FKs can be created.
            migrationBuilder.Sql(@"
UPDATE t SET DSItemID = NULL FROM DSTransaction t
    WHERE DSItemID = 0 OR DSTypeID IN (3, 4) OR NOT EXISTS (SELECT 1 FROM DSItem i WHERE i.ID = t.DSItemID);
UPDATE t SET DSItemSubID = NULL FROM DSTransaction t
    WHERE DSItemSubID = 0 OR DSTypeID IN (3, 4) OR NOT EXISTS (SELECT 1 FROM DSItemSub s WHERE s.ID = t.DSItemSubID);
UPDATE t SET TransferOutTransactionID = NULL FROM DSTransaction t
    WHERE TransferOutTransactionID = 0 OR NOT EXISTS (SELECT 1 FROM DSTransaction o WHERE o.ID = t.TransferOutTransactionID);");

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_DSAccountID",
                table: "DSTransaction",
                column: "DSAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_DSItemID",
                table: "DSTransaction",
                column: "DSItemID");

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_DSItemSubID",
                table: "DSTransaction",
                column: "DSItemSubID");

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_DSTypeID",
                table: "DSTransaction",
                column: "DSTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_MemberID_CreatedDateTime",
                table: "DSTransaction",
                columns: new[] { "MemberID", "CreatedDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_TransferOutTransactionID",
                table: "DSTransaction",
                column: "TransferOutTransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_DSTransaction_DSAccount_DSAccountID",
                table: "DSTransaction",
                column: "DSAccountID",
                principalTable: "DSAccount",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DSTransaction_DSItemSub_DSItemSubID",
                table: "DSTransaction",
                column: "DSItemSubID",
                principalTable: "DSItemSub",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DSTransaction_DSItem_DSItemID",
                table: "DSTransaction",
                column: "DSItemID",
                principalTable: "DSItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DSTransaction_DSTransaction_TransferOutTransactionID",
                table: "DSTransaction",
                column: "TransferOutTransactionID",
                principalTable: "DSTransaction",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DSTransaction_DSType_DSTypeID",
                table: "DSTransaction",
                column: "DSTypeID",
                principalTable: "DSType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DSTransaction_DSAccount_DSAccountID",
                table: "DSTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_DSTransaction_DSItemSub_DSItemSubID",
                table: "DSTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_DSTransaction_DSItem_DSItemID",
                table: "DSTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_DSTransaction_DSTransaction_TransferOutTransactionID",
                table: "DSTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_DSTransaction_DSType_DSTypeID",
                table: "DSTransaction");

            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_DSAccountID",
                table: "DSTransaction");

            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_DSItemID",
                table: "DSTransaction");

            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_DSItemSubID",
                table: "DSTransaction");

            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_DSTypeID",
                table: "DSTransaction");

            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_MemberID_CreatedDateTime",
                table: "DSTransaction");

            migrationBuilder.DropIndex(
                name: "IX_DSTransaction_TransferOutTransactionID",
                table: "DSTransaction");

            migrationBuilder.Sql(@"
UPDATE DSTransaction SET DSItemID = 0 WHERE DSItemID IS NULL;
UPDATE DSTransaction SET DSItemSubID = 0 WHERE DSItemSubID IS NULL;
UPDATE DSTransaction SET TransferOutTransactionID = 0 WHERE TransferOutTransactionID IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "DSItemSubID",
                table: "DSTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DSItemID",
                table: "DSTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TransferOutTransactionID",
                table: "DSTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "TransferOutTransactionID",
                table: "DSTransaction",
                newName: "DSTransferOutID");

            migrationBuilder.CreateIndex(
                name: "IX_DSTransaction_MemberID",
                table: "DSTransaction",
                column: "MemberID");
        }
    }
}
