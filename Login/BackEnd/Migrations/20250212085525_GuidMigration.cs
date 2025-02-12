using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginAPI.Migrations
{
    /// <inheritdoc />
    public partial class GuidMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropPrimaryKey("PK_user", "user");


            migrationBuilder.DropColumn("Id", "user");


            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "user",
                type: "nvarchar(450)",
                nullable: false,
                defaultValueSql: "NEWID()"
            );


            migrationBuilder.AddPrimaryKey("PK_user", "user", "Id");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "user",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
