using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OutOfOfficeHRApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbsenceReason",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceReason", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subdivision",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subdivision", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubdivisionID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PeoplePartnerID = table.Column<int>(type: "int", nullable: true),
                    OutOfOfficeBalance = table.Column<int>(type: "int", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_PeoplePartnerID",
                        column: x => x.PeoplePartnerID,
                        principalTable: "Employee",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Employee_Position_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Position",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Employee_Subdivision_SubdivisionID",
                        column: x => x.SubdivisionID,
                        principalTable: "Subdivision",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequest",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    AbsenceReasonID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_AbsenceReason_AbsenceReasonID",
                        column: x => x.AbsenceReasonID,
                        principalTable: "AbsenceReason",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectTypeID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Project_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_ProjectType_ProjectTypeID",
                        column: x => x.ProjectTypeID,
                        principalTable: "ProjectType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequest",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    LeaveRequestID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ApprovalRequest_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ApprovalRequest_LeaveRequest_LeaveRequestID",
                        column: x => x.LeaveRequestID,
                        principalTable: "LeaveRequest",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "AbsenceReason",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Illness" },
                    { 2, "Family Matter" },
                    { 3, "Official Matter" },
                    { 4, "Holiday" },
                    { 5, "Other" }
                });

            migrationBuilder.InsertData(
                table: "Position",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Employee" },
                    { 2, "HR Manager" },
                    { 3, "Project Manager" },
                    { 4, "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "ProjectType",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "CMS" },
                    { 2, "Mobile E-Commerce" },
                    { 3, "CRM" },
                    { 4, "ERP" }
                });

            migrationBuilder.InsertData(
                table: "Subdivision",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Software Development" },
                    { 2, "Quality Assurance" },
                    { 3, "User Experience/User Interface" },
                    { 4, "Product Management" },
                    { 5, "Customer Support" },
                    { 6, "Buissness Analysis" },
                    { 7, "Information Security" },
                    { 8, "IT Infractructure" },
                    { 9, "Data Management" },
                    { 10, "HR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequest_EmployeeID",
                table: "ApprovalRequest",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequest_LeaveRequestID",
                table: "ApprovalRequest",
                column: "LeaveRequestID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PeoplePartnerID",
                table: "Employee",
                column: "PeoplePartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PositionID",
                table: "Employee",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_SubdivisionID",
                table: "Employee",
                column: "SubdivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_AbsenceReasonID",
                table: "LeaveRequest",
                column: "AbsenceReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_EmployeeID",
                table: "LeaveRequest",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_EmployeeID",
                table: "Project",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectTypeID",
                table: "Project",
                column: "ProjectTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRequest");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "LeaveRequest");

            migrationBuilder.DropTable(
                name: "ProjectType");

            migrationBuilder.DropTable(
                name: "AbsenceReason");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Subdivision");
        }
    }
}
