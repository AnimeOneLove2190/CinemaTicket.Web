using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaTicket.DataAccess.Migrations
{
    public partial class createPosterNameFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterFileName",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterFileName",
                table: "Movies");
        }
    }
}
