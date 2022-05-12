using Microsoft.EntityFrameworkCore.Migrations;

namespace HakimsLivs.Data.Migrations
{
    public partial class OrderID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "OrderProducts",
                newName: "OrderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "OrderProducts",
                newName: "UserID");
        }
    }
}
