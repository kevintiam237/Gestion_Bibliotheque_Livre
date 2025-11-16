using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Bibliotheque_Livre.Migrations
{
    /// <inheritdoc />
    public partial class NewBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        // Liste des nouveaux livres
        var livresSupplementaires = new[]
        {
            // FANTASTIQUE / FANTASY (Catégorie 3)
            new { Titre = "Le Seigneur des Anneaux", AuteurId = 16, Pages = 1178, ISBN = "978-2-07-041580-1", CategorieId = 3, Annee = 1954, Exemplaires = 3 },
            new { Titre = "Le Hobbit", AuteurId = 16, Pages = 320, ISBN = "978-2-07-041579-5", CategorieId = 3, Annee = 1937, Exemplaires = 5 },
            new { Titre = "Harry Potter à l'école des sorciers", AuteurId = 17, Pages = 336, ISBN = "978-2-07-053607-0", CategorieId = 3, Annee = 1997, Exemplaires = 4 },
            new { Titre = "Harry Potter et la Chambre des Secrets", AuteurId = 17, Pages = 320, ISBN = "978-2-07-053608-7", CategorieId = 3, Annee = 1998, Exemplaires = 4 },
            new { Titre = "1984", AuteurId = 18, Pages = 328, ISBN = "978-2-07-036838-6", CategorieId = 2, Annee = 1949, Exemplaires = 6 },
            new { Titre = "La Ferme des Animaux", AuteurId = 18, Pages = 128, ISBN = "978-2-07-036839-3", CategorieId = 2, Annee = 1945, Exemplaires = 5 },

            // ROMAN (Catégorie 1)
            new { Titre = "Orgueil et Préjugés", AuteurId = 19, Pages = 448, ISBN = "978-2-07-040826-6", CategorieId = 1, Annee = 1813, Exemplaires = 3 },
            new { Titre = "Pour qui sonne le glas", AuteurId = 20, Pages = 528, ISBN = "978-2-07-036003-8", CategorieId = 1, Annee = 1940, Exemplaires = 2 },
            new { Titre = "Gatsby le Magnifique", AuteurId = 21, Pages = 256, ISBN = "978-2-07-036004-5", CategorieId = 1, Annee = 1925, Exemplaires = 4 },
            new { Titre = "Crime et Châtiment", AuteurId = 22, Pages = 624, ISBN = "978-2-07-040827-3", CategorieId = 1, Annee = 1866, Exemplaires = 3 },
            new { Titre = "Les Frères Karamazov", AuteurId = 22, Pages = 1248, ISBN = "978-2-07-040827-4", CategorieId = 1, Annee = 1880, Exemplaires = 2 },
            new { Titre = "Moby Dick", AuteurId = 23, Pages = 640, ISBN = "978-2-07-040828-0", CategorieId = 1, Annee = 1851, Exemplaires = 3 },
            new { Titre = "Oliver Twist", AuteurId = 24, Pages = 544, ISBN = "978-2-07-040829-7", CategorieId = 1, Annee = 1838, Exemplaires = 4 },
            new { Titre = "David Copperfield", AuteurId = 24, Pages = 864, ISBN = "978-2-07-040830-3", CategorieId = 1, Annee = 1850, Exemplaires = 2 },

            // THÉÂTRE / CLASSIQUE (Catégorie 1)
            new { Titre = "Hamlet", AuteurId = 25, Pages = 224, ISBN = "978-2-07-040831-0", CategorieId = 1, Annee = 1601, Exemplaires = 2 },
            new { Titre = "Roméo et Juliette", AuteurId = 25, Pages = 208, ISBN = "978-2-07-040832-7", CategorieId = 1, Annee = 1595, Exemplaires = 3 },

            // POLICIER / NOUVELLE (Catégorie 4)
            new { Titre = "Les Aventures d'Arthur Gordon Pym", AuteurId = 26, Pages = 192, ISBN = "978-2-07-040833-4", CategorieId = 4, Annee = 1838, Exemplaires = 1 },
            new { Titre = "La Machine à explorer le temps", AuteurId = 27, Pages = 144, ISBN = "978-2-07-053609-4", CategorieId = 2, Annee = 1895, Exemplaires = 3 },
            new { Titre = "La Guerre des mondes", AuteurId = 27, Pages = 288, ISBN = "978-2-07-053610-0", CategorieId = 2, Annee = 1898, Exemplaires = 3 },

            // SCIENCE-FICTION (Catégorie 2)
            new { Titre = "Fondation", AuteurId = 28, Pages = 256, ISBN = "978-2-07-040834-1", CategorieId = 2, Annee = 1951, Exemplaires = 4 },
            new { Titre = "2001 : L'Odyssée de l'espace", AuteurId = 29, Pages = 311, ISBN = "978-2-07-040835-8", CategorieId = 2, Annee = 1968, Exemplaires = 2 },

            // ROMAN CONTEMPORAIN (Catégorie 1)
            new { Titre = "Kafka sur le rivage", AuteurId = 30, Pages = 528, ISBN = "978-2-07-041581-8", CategorieId = 1, Annee = 2002, Exemplaires = 3 },
            new { Titre = "Norwegian Wood", AuteurId = 30, Pages = 384, ISBN = "978-2-07-041582-5", CategorieId = 1, Annee = 1987, Exemplaires = 2 }
        };

            int nextId = 29;

            foreach (var livre in livresSupplementaires)
            {
                migrationBuilder.InsertData(
                    table: "Livres",
                    columns: new[] { "Id", "Titre", "DatePublication", "AuteurId" },
                    values: new object[]
                    {
                        nextId,
                        livre.Titre,
                        new DateTime(livre.Annee, 1, 1),
                        livre.AuteurId
                    });

                migrationBuilder.InsertData(
                    table: "DetailsLivres",
                    columns: new[] { "ISBN", "NombrePages", "LivreId", "NombreExemplaires" },
                    values: new object[]
                    {
                        livre.ISBN,
                        livre.Pages,
                        nextId,
                        livre.Exemplaires
                    });

                migrationBuilder.InsertData(
                    table: "LivreCategories",
                    columns: new[] { "LivreId", "CategorieId" },
                    values: new object[] { nextId, livre.CategorieId });

                nextId++;
            }

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
