using BibliothequeApp.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Bibliotheque_Livre.Migrations
{
    /// <inheritdoc />
    public partial class AddLivres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Auteurs)
                BEGIN
                    INSERT INTO Auteurs (Nom, Prenom) VALUES 
                    ('Hugo', 'Victor'),
                    ('Zola', 'Émile'),
                    ('Flaubert', 'Gustave'),
                    ('Balzac', 'Honoré de'),
                    ('Dumas', 'Alexandre'),
                    ('Sand', 'George'),
                    ('Verne', 'Jules'),
                    ('Maupassant', 'Guy de'),
                    ('Proust', 'Marcel'),
                    ('Camus', 'Albert'),
                    ('Saint-Exupéry', 'Antoine de'),
                    ('Voltaire', ''),
                    ('Molière', ''),
                    ('Duras', 'Marguerite'),
                    ('Sartre', 'Jean-Paul')
                END
            ");

            // 2. Livres célèbres de la littérature française avec années réelles
            var livres = new[]
            {
                // ROMAN (Catégorie 1)
                new { Titre = "Les Misérables", AuteurId = 1, Pages = 1232, ISBN = "978-2-07-040623-1", CategorieId = 1, Annee = 1862 },
                new { Titre = "Germinal", AuteurId = 2, Pages = 554, ISBN = "978-2-07-040847-1", CategorieId = 1, Annee = 1885 },
                new { Titre = "Madame Bovary", AuteurId = 3, Pages = 432, ISBN = "978-2-07-040430-5", CategorieId = 1, Annee = 1857 },
                new { Titre = "Le Père Goriot", AuteurId = 4, Pages = 368, ISBN = "978-2-07-040327-8", CategorieId = 1, Annee = 1835 },
                new { Titre = "Les Trois Mousquetaires", AuteurId = 5, Pages = 768, ISBN = "978-2-07-040111-3", CategorieId = 1, Annee = 1844 },
                new { Titre = "La Mare au Diable", AuteurId = 6, Pages = 192, ISBN = "978-2-07-040715-3", CategorieId = 1, Annee = 1846 },
                new { Titre = "Bel-Ami", AuteurId = 8, Pages = 416, ISBN = "978-2-07-040620-0", CategorieId = 1, Annee = 1885 },
                new { Titre = "Du côté de chez Swann", AuteurId = 9, Pages = 528, ISBN = "978-2-07-040825-9", CategorieId = 1, Annee = 1913 },
                new { Titre = "L'Étranger", AuteurId = 10, Pages = 192, ISBN = "978-2-07-036002-1", CategorieId = 1, Annee = 1942 },
                new { Titre = "Candide", AuteurId = 12, Pages = 192, ISBN = "978-2-07-040598-2", CategorieId = 1, Annee = 1759 },

                // SCIENCE-FICTION (Catégorie 2)
                new { Titre = "Vingt Mille Lieues sous les mers", AuteurId = 7, Pages = 448, ISBN = "978-2-07-050704-4", CategorieId = 2, Annee = 1870 },
                new { Titre = "Le Tour du monde en quatre-vingts jours", AuteurId = 7, Pages = 320, ISBN = "978-2-07-050705-1", CategorieId = 2, Annee = 1873 },
                new { Titre = "Voyage au centre de la Terre", AuteurId = 7, Pages = 384, ISBN = "978-2-07-050706-8", CategorieId = 2, Annee = 1864 },
                new { Titre = "De la Terre à la Lune", AuteurId = 7, Pages = 288, ISBN = "978-2-07-050707-5", CategorieId = 2, Annee = 1865 },
                new { Titre = "L'Île mystérieuse", AuteurId = 7, Pages = 672, ISBN = "978-2-07-050708-2", CategorieId = 2, Annee = 1874 },

                // FANTASTIQUE (Catégorie 3)
                new { Titre = "Notre-Dame de Paris", AuteurId = 1, Pages = 940, ISBN = "978-2-07-040624-8", CategorieId = 3, Annee = 1831 },
                new { Titre = "Le Horla", AuteurId = 8, Pages = 128, ISBN = "978-2-07-040622-4", CategorieId = 3, Annee = 1887 },
                new { Titre = "La Peau de chagrin", AuteurId = 4, Pages = 384, ISBN = "978-2-07-040329-2", CategorieId = 3, Annee = 1831 },
                new { Titre = "Contes fantastiques", AuteurId = 4, Pages = 256, ISBN = "978-2-07-040330-8", CategorieId = 3, Annee = 1832 },

                // POLICIER (Catégorie 4)
                new { Titre = "Le Comte de Monte-Cristo", AuteurId = 5, Pages = 1312, ISBN = "978-2-07-040112-0", CategorieId = 4, Annee = 1844 },
                new { Titre = "Une vie", AuteurId = 8, Pages = 368, ISBN = "978-2-07-040621-7", CategorieId = 4, Annee = 1883 },
                new { Titre = "Les Mystères de Paris", AuteurId = 1, Pages = 896, ISBN = "978-2-07-040625-5", CategorieId = 4, Annee = 1843 },

                // BIOGRAPHIE (Catégorie 5)
                new { Titre = "Les Confessions", AuteurId = 12, Pages = 608, ISBN = "978-2-07-040756-6", CategorieId = 5, Annee = 1782 },
                new { Titre = "Mémoires d'outre-tombe", AuteurId = 1, Pages = 1024, ISBN = "978-2-07-040626-2", CategorieId = 5, Annee = 1849 },
                new { Titre = "L'Âge d'homme", AuteurId = 15, Pages = 224, ISBN = "978-2-07-032352-7", CategorieId = 5, Annee = 1939 },

                // HISTOIRE (Catégorie 6)
                new { Titre = "Quatre-vingt-treize", AuteurId = 1, Pages = 512, ISBN = "978-2-07-040627-9", CategorieId = 6, Annee = 1874 },
                new { Titre = "La Débâcle", AuteurId = 2, Pages = 612, ISBN = "978-2-07-040848-8", CategorieId = 6, Annee = 1892 },
                new { Titre = "Histoire de la Révolution française", AuteurId = 2, Pages = 704, ISBN = "978-2-07-040849-5", CategorieId = 6, Annee = 1865 }
            };


            // 3. Ajouter des livres à la base de données
            for (int i = 0; i < livres.Length; i++)
            {
                var livre = livres[i];

                // Insérer le livre
                migrationBuilder.InsertData(
                    table: "Livres",
                    columns: new[] { "Titre", "DatePublication", "AuteurId" },
                    values: new object[] {
                        livre.Titre,
                        new DateTime(livre.Annee, 1, 1),
                        livre.AuteurId
                    });

                // Insérer les détails du livre
                migrationBuilder.InsertData(
                    table: "DetailsLivres",
                    columns: new[] { "ISBN", "NombrePages", "LivreId" },
                    values: new object[] {
                        livre.ISBN,
                        livre.Pages,
                        i + 1
                    });

                // Associer à la catégorie
                migrationBuilder.InsertData(
                    table: "LivreCategories",
                    columns: new[] { "LivreId", "CategorieId" },
                    values: new object[] { i + 1, livre.CategorieId });
            }
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
