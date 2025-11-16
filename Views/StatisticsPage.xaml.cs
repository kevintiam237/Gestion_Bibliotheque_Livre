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
using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Bibliotheque_Livre.Views
{
    /// <summary>
    /// Logique d'interaction pour StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : UserControl
    {
        private ResourceManager resourceManager;

        public StatisticsPage()
        {
            // Initialise le gestionnaire de ressources
            resourceManager = new ResourceManager(
                "Gestion_Bibliotheque_Livre.Properties.Resources",
                typeof(StatisticsPage).Assembly);

            InitializeComponent();

            // Met à jour l'interface d'abord
            UpdateUIWithResources();

            // Charge les statistiques
            ChargerStatistiques();
        }


        public void ChargerStatistiques()
        {
            try
            {
                using var ctx = new DbContextBibliotheque();

                // ==================== COMPTEURS GLOBAUX ====================
                int nbAuteurs = ctx.Auteurs.Count();
                int nbLivres = ctx.Livres.Count();
                int nbCategories = ctx.Categories.Count();

                // Formatage selon la culture courante (1000 → 1 000 en FR, 1,000 en EN)
                StatAuthorsValue.Text = nbAuteurs.ToString("N0", CultureInfo.CurrentUICulture);
                StatBooksValue.Text = nbLivres.ToString("N0", CultureInfo.CurrentUICulture);
                StatCategoriesValue.Text = nbCategories.ToString("N0", CultureInfo.CurrentUICulture);

                // ==================== AUTEUR AVEC LE PLUS DE LIVRES ====================
                var auteurTop = ctx.Auteurs
                    .Include(a => a.Livres) // Inclure les livres pour le comptage
                    .Select(a => new
                    {
                        a.Nom,
                        a.Prenom,
                        Count = a.Livres.Count
                    })
                    .OrderByDescending(a => a.Count)
                    .ThenBy(a => a.Nom)
                    .FirstOrDefault();

                if (auteurTop != null && auteurTop.Count > 0)
                {
                    string livresText = auteurTop.Count == 1
                        ? (resourceManager.GetString("OneBook") ?? "livre")
                        : (resourceManager.GetString("ManyBooks") ?? "livres");

                    InfoAuthorValue.Text = $"{auteurTop.Prenom} {auteurTop.Nom} ({auteurTop.Count} {livresText})";
                }
                else
                {
                    InfoAuthorValue.Text = resourceManager.GetString("NoData") ?? "-";
                }

                // ==================== CATÉGORIE LA PLUS POPULAIRE ====================
                var categorieTop = ctx.Categories
                    .Include(c => c.LivreCategories)
                    .Select(c => new
                    {
                        c.Nom,
                        Count = c.LivreCategories.Count
                    })
                    .OrderByDescending(c => c.Count)
                    .ThenBy(c => c.Nom)
                    .FirstOrDefault();

                if (categorieTop != null && categorieTop.Count > 0)
                {
                    string livresText = categorieTop.Count == 1
                        ? (resourceManager.GetString("OneBook") ?? "livre")
                        : (resourceManager.GetString("ManyBooks") ?? "livres");

                    InfoCategoryValue.Text = $"{categorieTop.Nom} ({categorieTop.Count} {livresText})";
                }
                else
                {
                    InfoCategoryValue.Text = resourceManager.GetString("NoData") ?? "-";
                }

                // ==================== DERNIER LIVRE AJOUTÉ ====================
                var dernierLivre = ctx.Livres
                    .Include(l => l.Auteur)
                    .OrderByDescending(l => l.Id)
                    .FirstOrDefault();

                if (dernierLivre != null)
                {
                    // Formatage de la date selon la culture courante
                    string dateStr = dernierLivre.DatePublication.ToString("d", CultureInfo.CurrentUICulture);

                    // Affichage avec auteur si disponible
                    if (dernierLivre.Auteur != null)
                    {
                        InfoLastBookValue.Text = $"{dernierLivre.Titre} — {dernierLivre.Auteur.Prenom} {dernierLivre.Auteur.Nom} ({dateStr})";
                    }
                    else
                    {
                        InfoLastBookValue.Text = $"{dernierLivre.Titre} ({dateStr})";
                    }
                }
                else
                {
                    InfoLastBookValue.Text = resourceManager.GetString("NoData") ?? "-";
                }

            }
            catch (Exception ex)
            {
                // Log l'erreur sans faire crasher l'application
                System.Diagnostics.Debug.WriteLine($"Erreur ChargerStatistiques : {ex.Message}");

                // Afficher des valeurs par défaut en cas d'erreur
                StatAuthorsValue.Text = "0";
                StatBooksValue.Text = "0";
                StatCategoriesValue.Text = "0";
                InfoAuthorValue.Text = resourceManager.GetString("ErrorLoadingData") ?? "Erreur";
                InfoCategoryValue.Text = resourceManager.GetString("ErrorLoadingData") ?? "Erreur";
                InfoLastBookValue.Text = resourceManager.GetString("ErrorLoadingData") ?? "Erreur";
            }
        }

        public void ApplyLanguage()
        {
            UpdateUIWithResources();
            ChargerStatistiques();
        }

       
        public void ActualiserDonnees()
        {
            ChargerStatistiques();
        }

        
        public void Cleanup()
        {
            
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