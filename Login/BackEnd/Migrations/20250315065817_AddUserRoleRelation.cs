using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user",
                newName: "user_id");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "user",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "user",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "role_id",
                table: "user",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_Roles_role_id",
                table: "user",
                column: "role_id",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_Roles_role_id",
                table: "user");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_user_role_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "user",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "user",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "user",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
