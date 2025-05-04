using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleKeys",
                table: "RoleKeys");

            migrationBuilder.DropIndex(
                name: "IX_RoleKeys_RoleId",
                table: "RoleKeys");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoleUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoleKeys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleKeys",
                table: "RoleKeys",
                columns: new[] { "RoleId", "KeyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleKeys",
                table: "RoleKeys");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RoleUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RoleKeys",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleKeys",
                table: "RoleKeys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleKeys_RoleId",
                table: "RoleKeys",
                column: "RoleId");
        }
    }
}
