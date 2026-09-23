using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajProje.WebApi.Migrations
{
    public partial class mig11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTask_Chefs_ChefId",
                table: "EmployeeTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeTask",
                table: "EmployeeTask");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeTask_ChefId",
                table: "EmployeeTask");

            migrationBuilder.DropColumn(
                name: "ChefId",
                table: "EmployeeTask");

            migrationBuilder.RenameTable(
                name: "EmployeeTask",
                newName: "EmployeeTasks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeTasks",
                table: "EmployeeTasks",
                column: "EmployeeTaskId");

            migrationBuilder.CreateTable(
                name: "EmployeeTaskChefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeTaskId = table.Column<int>(type: "int", nullable: false),
                    ChefId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTaskChefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskChefs_Chefs_ChefId",
                        column: x => x.ChefId,
                        principalTable: "Chefs",
                        principalColumn: "ChefId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeTaskChefs_EmployeeTasks_EmployeeTaskId",
                        column: x => x.EmployeeTaskId,
                        principalTable: "EmployeeTasks",
                        principalColumn: "EmployeeTaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskChefs_ChefId",
                table: "EmployeeTaskChefs",
                column: "ChefId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTaskChefs_EmployeeTaskId",
                table: "EmployeeTaskChefs",
                column: "EmployeeTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeTaskChefs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeTasks",
                table: "EmployeeTasks");

            migrationBuilder.RenameTable(
                name: "EmployeeTasks",
                newName: "EmployeeTask");

            migrationBuilder.AddColumn<int>(
                name: "ChefId",
                table: "EmployeeTask",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeTask",
                table: "EmployeeTask",
                column: "EmployeeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTask_ChefId",
                table: "EmployeeTask",
                column: "ChefId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTask_Chefs_ChefId",
                table: "EmployeeTask",
                column: "ChefId",
                principalTable: "Chefs",
                principalColumn: "ChefId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
