using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSA.OrderService.Migrations.OrderStateDb
{
    /// <inheritdoc />
    public partial class AddOutboxState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxState",
                table: "OutboxState");

            migrationBuilder.RenameTable(
                name: "OutboxState",
                newName: "OutboxStates");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxStates",
                newName: "IX_OutboxStates_Created");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxStates",
                table: "OutboxStates",
                column: "OutboxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxStates",
                table: "OutboxStates");

            migrationBuilder.RenameTable(
                name: "OutboxStates",
                newName: "OutboxState");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxStates_Created",
                table: "OutboxState",
                newName: "IX_OutboxState_Created");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxState",
                table: "OutboxState",
                column: "OutboxId");
        }
    }
}
