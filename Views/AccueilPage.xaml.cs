using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Bibliotheque_Livre.Views
{
    /// <summary>
    /// Logique d'interaction pour AccueilPage.xaml
    /// </summary>
    public partial class AccueilPage : UserControl
    {
        private ResourceManager resourceManager;
        private DbContextBibliotheque _context;

        public AccueilPage()
        {
            InitializeComponent();

            // Initialiser le gestionnaire de ressources
            resourceManager = new ResourceManager(
                "Gestion_Bibliotheque_Livre.Properties.Resources",
                typeof(MainWindow).Assembly);

            // Initialiser le contexte de base de données
            _context = new DbContextBibliotheque();

            // Mettre à jour l'interface avec les ressources
            UpdateUIWithResources();

            // Charger et afficher les statistiques
            ChargerStatistiques();
        }

        private void ChargerStatistiques()
        {
            try
            {
                // Compter les éléments dans la base de données
                int nombreLivres = _context.Livres.Count();
                int nombreAuteurs = _context.Auteurs.Count();
                int nombreCategories = _context.Categories.Count();

                // Construire le message de statut avec les statistiques
                string statusMessage;

                if (nombreLivres == 0 && nombreAuteurs == 0)
                {
                    // Base de données vide
                    statusMessage = resourceManager.GetString("StatusEmpty") ??
                        "📚 Votre bibliothèque est vide. Commencez par ajouter des auteurs et des livres !";
                }
                else
                {
                    // Afficher les statistiques
                    string livresText = nombreLivres == 1 ?
                        (resourceManager.GetString("OneBook") ?? "livre") :
                        (resourceManager.GetString("ManyBooks") ?? "livres");

                    string auteursText = nombreAuteurs == 1 ?
                        (resourceManager.GetString("OneAuthor") ?? "auteur") :
                        (resourceManager.GetString("ManyAuthors") ?? "auteurs");

                    string categoriesText = nombreCategories == 1 ?
                        (resourceManager.GetString("OneCategory") ?? "catégorie") :
                        (resourceManager.GetString("ManyCategories") ?? "catégories");

                    statusMessage = string.Format(
                        resourceManager.GetString("StatusWithData") ??
                        "📊 Votre bibliothèque contient actuellement {0} {1}, {2} {3} et {4} {5}.",
                        nombreLivres, livresText,
                        nombreAuteurs, auteursText,
                        nombreCategories, categoriesText);
                }

                StatusMessageTextBlock.Text = statusMessage;

                // Optionnel : Afficher le dernier livre ajouté
                AfficherDernierLivre();
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = resourceManager.GetString("StatusError") ??
                    "❌ Erreur lors du chargement des statistiques.";

                Console.WriteLine($"Erreur ChargerStatistiques : {ex.Message}");
            }
        }

    
        private void AfficherDernierLivre()
        {
            try
            {
                var dernierLivre = _context.Livres
                    .Include(l => l.Auteur)
                    .OrderByDescending(l => l.Id)
                    .FirstOrDefault();

                if (dernierLivre != null)
                {
                    string dateFormatee = dernierLivre.DatePublication.ToString("d", CultureInfo.CurrentUICulture);
                    string message = string.Format(
                        resourceManager.GetString("LastBookInfo") ??
                        "\n\n✨ Dernier livre ajouté : \"{0}\" par {1} {2}",
                        dernierLivre.Titre,
                        dernierLivre.Auteur?.Prenom ?? "",
                        dernierLivre.Auteur?.Nom ?? "");

                    StatusMessageTextBlock.Text += message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur AfficherDernierLivre : {ex.Message}");
            }
        }

      
        public void ActualiserDonnees()
        {
            ChargerStatistiques();
        }

      
        public void ChangerLangue()
        {
            UpdateUIWithResources();
            ChargerStatistiques();
        }

     
        public void Cleanup()
        {
            _context?.Dispose();
        }
    
         public void UpdateUIWithResources() 
         { 
        
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            WelcomeTitleTextBlock.Text = $"📚 {resourceManager.GetString("WelcomeTitle")}";
            WelcomeSubtitleTextBlock.Text = resourceManager.GetString("WelcomeSubtitle");
            ExploreTitleTextBlock.Text = $"🌟 {resourceManager.GetString("ExploreTitle")}";
            ExploreDescriptionTextBlock.Text = resourceManager.GetString("ExploreDescription");
            Feature1TitleTextBlock.Text = resourceManager.GetString("Feature1Title");
            Feature1DescriptionTextBlock.Text = resourceManager.GetString("Feature1Description");
            Feature2TitleTextBlock.Text = resourceManager.GetString("Feature2Title");
            Feature2DescriptionTextBlock.Text = resourceManager.GetString("Feature2Description");
            Feature3TitleTextBlock.Text = resourceManager.GetString("Feature3Title");
            Feature3DescriptionTextBlock.Text = resourceManager.GetString("Feature3Description");
            QuoteTextTextBlock.Text = resourceManager.GetString("QuoteText");
            QuoteAuthorTextBlock.Text = resourceManager.GetString("QuoteAuthor");
            NavigationTitleTextBlock.Text = $"🎯 {resourceManager.GetString("NavigationTitle")}";
            NavigationDescriptionTextBlock.Text = resourceManager.GetString("NavigationDescription");
            StatusMessageTextBlock.Text = resourceManager.GetString("StatusMessage");

         }
    }
}
    

