using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajProje.WebApi.Migrations
{
    public partial class mig9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            /*
                migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Abouts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
            */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Abouts");
        }
    }
}
