using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutOfOfficeHRApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedProjectToEmployeeAndRelationsBetween : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequest_AbsenceReason_AbsenceReasonID",
                table: "LeaveRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_EmployeeID",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Project",
                newName: "ProjectManagerID");

            migrationBuilder.RenameIndex(
                name: "IX_Project_EmployeeID",
                table: "Project",
                newName: "IX_Project_ProjectManagerID");

            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ProjectID",
                table: "Employee",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Project_ProjectID",
                table: "Employee",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequest_AbsenceReason_AbsenceReasonID",
                table: "LeaveRequest",
                column: "AbsenceReasonID",
                principalTable: "AbsenceReason",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_ProjectManagerID",
                table: "Project",
                column: "ProjectManagerID",
                principalTable: "Employee",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Project_ProjectID",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequest_AbsenceReason_AbsenceReasonID",
                table: "LeaveRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_ProjectManagerID",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Employee_ProjectID",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "ProjectManagerID",
                table: "Project",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectManagerID",
                table: "Project",
                newName: "IX_Project_EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequest_AbsenceReason_AbsenceReasonID",
                table: "LeaveRequest",
                column: "AbsenceReasonID",
                principalTable: "AbsenceReason",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_EmployeeID",
                table: "Project",
                column: "EmployeeID",
                principalTable: "Employee",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
