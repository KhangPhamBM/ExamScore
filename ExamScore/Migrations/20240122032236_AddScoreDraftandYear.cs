using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamScore.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreDraftandYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearId",
                table: "ScoreDrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreDrafts_YearId",
                table: "ScoreDrafts",
                column: "YearId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreDrafts_SchoolYears_YearId",
                table: "ScoreDrafts",
                column: "YearId",
                principalTable: "SchoolYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreDrafts_SchoolYears_YearId",
                table: "ScoreDrafts");

            migrationBuilder.DropIndex(
                name: "IX_ScoreDrafts_YearId",
                table: "ScoreDrafts");

            migrationBuilder.DropColumn(
                name: "YearId",
                table: "ScoreDrafts");
        }
    }
}
