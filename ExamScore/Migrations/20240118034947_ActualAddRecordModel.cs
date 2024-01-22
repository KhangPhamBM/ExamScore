using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamScore.Migrations
{
    /// <inheritdoc />
    public partial class ActualAddRecordModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SBD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Toan = table.Column<double>(type: "float", nullable: true),
                    Van = table.Column<double>(type: "float", nullable: true),
                    Ly = table.Column<double>(type: "float", nullable: true),
                    Sinh = table.Column<double>(type: "float", nullable: true),
                    NgoaiNgu = table.Column<double>(type: "float", nullable: true),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hoa = table.Column<double>(type: "float", nullable: true),
                    LichSu = table.Column<double>(type: "float", nullable: true),
                    DiaLy = table.Column<double>(type: "float", nullable: true),
                    GDCD = table.Column<double>(type: "float", nullable: true),
                    MaTinh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordModels");
        }
    }
}
