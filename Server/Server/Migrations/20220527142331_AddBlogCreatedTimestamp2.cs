using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WX.Migrations
{
    public partial class AddBlogCreatedTimestamp2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_playerAccount",
                table: "playerAccount");

            migrationBuilder.RenameTable(
                name: "playerAccount",
                newName: "playerAccountTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_playerAccountTable",
                table: "playerAccountTable",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_playerAccountTable",
                table: "playerAccountTable");

            migrationBuilder.RenameTable(
                name: "playerAccountTable",
                newName: "playerAccount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_playerAccount",
                table: "playerAccount",
                column: "Id");
        }
    }
}
