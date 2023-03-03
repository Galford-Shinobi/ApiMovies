using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiMovies.Common.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRoadImagBinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ByteImagen",
                table: "Pelicula",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByteImagen",
                table: "Pelicula");
        }
    }
}
