using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserAccessMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserExtraKeys_user_UserId1",
                table: "UserExtraKeys");

            migrationBuilder.DropIndex(
                name: "IX_UserExtraKeys_UserId1",
                table: "UserExtraKeys");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserExtraKeys");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserExtraKeys",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserExtraKeys_UserId",
                table: "UserExtraKeys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserExtraKeys_user_UserId",
                table: "UserExtraKeys",
                column: "UserId",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserExtraKeys_user_UserId",
                table: "UserExtraKeys");

            migrationBuilder.DropIndex(
                name: "IX_UserExtraKeys_UserId",
                table: "UserExtraKeys");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserExtraKeys",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserExtraKeys",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExtraKeys_UserId1",
                table: "UserExtraKeys",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserExtraKeys_user_UserId1",
                table: "UserExtraKeys",
                column: "UserId1",
                principalTable: "user",
                principalColumn: "user_id");
        }
    }
}
