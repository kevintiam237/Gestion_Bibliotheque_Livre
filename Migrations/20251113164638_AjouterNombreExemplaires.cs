using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Bibliotheque_Livre.Migrations
{
    /// <inheritdoc />
    public partial class AjouterNombreExemplaires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NombreExemplaires",
                table: "DetailsLivres",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreExemplaires",
                table: "DetailsLivres");
        }
    }
}
