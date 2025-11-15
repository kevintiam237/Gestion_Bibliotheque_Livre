using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gestion_Bibliotheque_Livre.Views
{
    /// <summary>
    /// Logique d'interaction pour StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage
    {
        private ResourceManager resourceManager;

        // Stocke la clé de ressource du message d'erreur actuel (ex: "ErrorBookTitle")
        private string? currentErrorKey;

        // Stocke un paramètre additionnel éventuel pour le message d'erreur (ex: le message d'exception)
        private string? currentErrorParams;
        public StatisticsPage()
        {

            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(MainWindow).Assembly);
            InitializeComponent();
             UpdateUIWithResources();
            ChargerStatistiques();
        }

        public void ChargerStatistiques()
        {
            try
            {
                using var ctx = new DbContextBibliotheque();

                // Compteurs globaux
                int nbAuteurs = ctx.Auteurs.Count();
                int nbLivres = ctx.Livres.Count();
                int nbCategories = ctx.Categories.Count();

                StatAuthorsValue.Text = nbAuteurs.ToString("N0", CultureInfo.CurrentUICulture);
                StatBooksValue.Text = nbLivres.ToString("N0", CultureInfo.CurrentUICulture);
                StatCategoriesValue.Text = nbCategories.ToString("N0", CultureInfo.CurrentUICulture);

                // Auteur avec le plus de livres
                var auteurTop = ctx.Auteurs
                    .Select(a => new { a.Nom, a.Prenom, Count = a.Livres.Count })
                    .OrderByDescending(a => a.Count)
                    .ThenBy(a => a.Nom)
                    .FirstOrDefault();

                InfoAuthorValue.Text =
                    auteurTop != null && auteurTop.Count > 0
                        ? $"{auteurTop.Nom} {auteurTop.Prenom} ({auteurTop.Count})"
                        : (resourceManager.GetString("NoData") ?? "-");

                // Catégorie la plus populaire
                var categorieTop = ctx.Categories
                    .Select(c => new { c.Nom, Count = c.LivreCategories.Count })
                    .OrderByDescending(c => c.Count)
                    .ThenBy(c => c.Nom)
                    .FirstOrDefault();

                InfoCategoryValue.Text =
                    categorieTop != null && categorieTop.Count > 0
                        ? $"{categorieTop.Nom} ({categorieTop.Count})"
                        : (resourceManager.GetString("NoData") ?? "-");

                // Dernier livre ajouté (par Id décroissant)
                var dernierLivre = ctx.Livres
                    .OrderByDescending(l => l.Id)
                    .FirstOrDefault();

                if (dernierLivre != null)
                {
                    // Utiliser la propriété formatée dépendante de la culture
                    var temp = new Livre
                    {
                        Id = dernierLivre.Id,
                        Titre = dernierLivre.Titre,
                        DatePublication = dernierLivre.DatePublication
                    };
                    string dateStr = temp.DatePublicationFormatee;
                    InfoLastBookValue.Text = $"{dernierLivre.Titre} — {dateStr}";
                }
                else
                {
                    InfoLastBookValue.Text = resourceManager.GetString("NoData") ?? "-";
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur inattendue sur les stats, on affiche dans la barre d'erreur
                //AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }
        public void UpdateUIWithResources()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            //// Onglet Statistiques
            StatTitle.Text = resourceManager.GetString("LibraryStatistics");
            StatAuthorsTitle.Text = resourceManager.GetString("AuthorsCount");
            StatBooksTitle.Text = resourceManager.GetString("BooksCount");
            StatCategoriesTitle.Text = resourceManager.GetString("NumberOfCategories");
            InfoTitle.Text = $"ℹ️ {resourceManager.GetString("GeneralInformation")}";
            InfoAuthor.Text = $"👉 {resourceManager.GetString("AuthorWithMostbooks")}:";
            InfoCategory.Text = $"👉 {resourceManager.GetString("MostPopularCategory")}:";
            InfoLastBook.Text = $"👉 {resourceManager.GetString("LastBookAdded")}:";
            StatSubtitle.Text = resourceManager.GetString("LibraryStat");
        }
    }
}