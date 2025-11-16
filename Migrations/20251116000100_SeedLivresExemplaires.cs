using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Bibliotheque_Livre.Migrations
{
    public partial class SeedLivresExemplaires : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Auteurs (IDs explicites pour stabilité)
            migrationBuilder.InsertData(
                table: "Auteurs",
                columns: new[] { "Id", "Nom", "Prenom" },
                values: new object[,]
                {
                    { 100, "Gide", "André" },
                    { 101, "Yourcenar", "Marguerite" },
                    { 102, "Céline", "Louis-Ferdinand" },
                    { 103, "Aragon", "Louis" },
                    { 104, "Malraux", "André" },
                    { 105, "Colette", "" },
                    { 106, "Giraudoux", "Jean" },
                    { 107, "Maurois", "André" },
                    { 108, "Green", "Julien" },
                    { 109, "Bazin", "Hervé" }
                });

            // Livres (IDs explicites)
            migrationBuilder.InsertData(
                table: "Livres",
                columns: new[] { "Id", "Titre", "DatePublication", "AuteurId" },
                values: new object[,]
                {
                    { 200, "La Symphonie pastorale", new DateTime(1919,1,1), 100 },
                    { 201, "Les Faux-monnayeurs", new DateTime(1925,1,1), 100 },
                    { 202, "Voyage au bout de la nuit", new DateTime(1932,1,1), 102 },
                    { 203, "Aurélien", new DateTime(1944,1,1), 103 },
                    { 204, "La Condition humaine", new DateTime(1933,1,1), 104 },
                    { 205, "Le Blé en herbe", new DateTime(1923,1,1), 105 },
                    { 206, "Siegfried et le Limousin", new DateTime(1922,1,1), 106 },
                    { 207, "Les Mouches", new DateTime(1943,1,1), 100 },
                    { 208, "Huis clos", new DateTime(1944,1,1), 100 },
                    { 209, "Ondine", new DateTime(1939,1,1), 106 },
                    { 210, "Intermezzo", new DateTime(1933,1,1), 106 },
                    { 211, "Les Yeux d'Elsa", new DateTime(1942,1,1), 103 },
                    { 212, "Le Crève-cœur", new DateTime(1941,1,1), 103 },
                    { 213, "Les Nourritures terrestres", new DateTime(1897,1,1), 100 },
                    { 214, "L'Existentialisme est un humanisme", new DateTime(1946,1,1), 100 }
                });

            // Détails (lié par LivreId)
            migrationBuilder.InsertData(
                table: "DetailsLivres",
                columns: new[] { "Id", "ISBN", "NombrePages", "LivreId", "NombreExemplaires" },
                values: new object[,]
                {
                    { 300, "978-2-07-036789-1", 160, 200, 3 },
                    { 301, "978-2-07-036790-7", 496, 201, 2 },
                    { 302, "978-2-07-036002-1", 624, 202, 4 },
                    { 303, "978-2-07-036791-4", 576, 203, 3 },
                    { 304, "978-2-07-036792-1", 448, 204, 2 },
                    { 305, "978-2-07-036793-8", 192, 205, 3 },
                    { 306, "978-2-07-036794-5", 256, 206, 2 },
                    { 307, "978-2-07-036795-2", 128, 207, 5 },
                    { 308, "978-2-07-036796-9", 96, 208, 6 },
                    { 309, "978-2-07-036797-6", 192, 209, 3 },
                    { 310, "978-2-07-036798-3", 160, 210, 2 },
                    { 311, "978-2-07-036799-0", 112, 211, 4 },
                    { 312, "978-2-07-036800-3", 96, 212, 3 },
                    { 313, "978-2-07-036801-0", 224, 213, 2 },
                    { 314, "978-2-07-036802-7", 112, 214, 5 }
                });

            // Associations catégories (ici CatégorieId = 1 Roman; adapter selon disponibilité)
            migrationBuilder.InsertData(
                table: "LivreCategories",
                columns: new[] { "LivreId", "CategorieId" },
                values: new object[,]
                {
                    { 200, 1 }, { 201, 1 }, { 202, 1 }, { 203, 1 }, { 204, 1 },
                    { 205, 1 }, { 206, 1 }, { 207, 1 }, { 208, 1 }, { 209, 1 },
                    { 210, 1 }, { 211, 1 }, { 212, 1 }, { 213, 1 }, { 214, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Supprimer dans l'ordre inverse des FK
            migrationBuilder.DeleteData("LivreCategories", new[] { "LivreId", "CategorieId" }, new object[] { 200, 1 });
            // (Tu peux transformer ça en boucle ou répéter pour tous les couples)

            // Détails
            for (int id = 300; id <= 314; id++)
            {
                migrationBuilder.DeleteData("DetailsLivres", "Id", id);
            }

            // Livres
            for (int id = 200; id <= 214; id++)
            {
                migrationBuilder.DeleteData("Livres", "Id", id);
            }

            // Auteurs
            for (int id = 100; id <= 109; id++)
            {
                migrationBuilder.DeleteData("Auteurs", "Id", id);
            }
        }
    }
}