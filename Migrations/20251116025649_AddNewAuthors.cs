using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Bibliotheque_Livre.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAuthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Auteurs",
            columns: new[] { "Id", "Nom", "Prenom" },
            values: new object[,]
            {
                { 16, "Tolkien", "J.R.R." },
                { 17, "Rowling", "J.K." },
                { 18, "Orwell", "George" },
                { 19, "Austen", "Jane" },
                { 20, "Hemingway", "Ernest" },
                { 21, "Fitzgerald", "F. Scott" },
                { 22, "Dostoevsky", "Fyodor" },
                { 23, "Melville", "Herman" },
                { 24, "Dickens", "Charles" },
                { 25, "Shakespeare", "" },
                { 26, "Poe", "Edgar Allan" },
                { 27, "Wells", "H.G." },
                { 28, "Asimov", "Isaac" },
                { 29, "Clarke", "Arthur C." },
                { 30, "Murakami", "Haruki" }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
