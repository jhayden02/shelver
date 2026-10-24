using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shelver.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToMillimeters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WidthCm",
                table: "FloorPlans",
                newName: "WidthMm");

            migrationBuilder.RenameColumn(
                name: "HeightCm",
                table: "FloorPlans",
                newName: "HeightMm");

            migrationBuilder.RenameColumn(
                name: "OffsetCm",
                table: "Doors",
                newName: "OffsetMm");

            // Renaming keeps the stored numbers, so existing centimeter values are scaled to
            // millimeters here. Wall coordinates change units without changing column names.
            migrationBuilder.Sql("UPDATE FloorPlans SET WidthMm = WidthMm * 10, HeightMm = HeightMm * 10;");
            migrationBuilder.Sql("UPDATE Walls SET StartX = StartX * 10, StartY = StartY * 10, EndX = EndX * 10, EndY = EndY * 10;");
            migrationBuilder.Sql("UPDATE Doors SET OffsetMm = OffsetMm * 10;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Values go back to centimeters before the columns are renamed. Integer division
            // drops any sub-centimeter remainder.
            migrationBuilder.Sql("UPDATE FloorPlans SET WidthMm = WidthMm / 10, HeightMm = HeightMm / 10;");
            migrationBuilder.Sql("UPDATE Walls SET StartX = StartX / 10, StartY = StartY / 10, EndX = EndX / 10, EndY = EndY / 10;");
            migrationBuilder.Sql("UPDATE Doors SET OffsetMm = OffsetMm / 10;");

            migrationBuilder.RenameColumn(
                name: "WidthMm",
                table: "FloorPlans",
                newName: "WidthCm");

            migrationBuilder.RenameColumn(
                name: "HeightMm",
                table: "FloorPlans",
                newName: "HeightCm");

            migrationBuilder.RenameColumn(
                name: "OffsetMm",
                table: "Doors",
                newName: "OffsetCm");
        }
    }
}
