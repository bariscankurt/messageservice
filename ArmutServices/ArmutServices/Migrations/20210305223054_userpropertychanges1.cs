using Microsoft.EntityFrameworkCore.Migrations;

namespace ArmutServices.Migrations
{
    public partial class userpropertychanges1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockedUsers",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockedUsers",
                table: "AspNetUsers");
        }
    }
}
