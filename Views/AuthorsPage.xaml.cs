using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace Gestion_Bibliotheque_Livre.Views
{
    public partial class AuthorsPage : UserControl

      
    {
        private ResourceManager resourceManager;

        public AuthorsPage()
        {
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(AuthorsPage).Assembly);
            InitializeComponent();
            UpdateUIWithResources();
            ChargerAuthors();

        }
            
        public void ChargerAuthors()
        {
            using (var ListeDesAutheurs = new DbContextBibliotheque())
            {
                var authors = ListeDesAutheurs.Auteurs
                    .Include(a => a.Livres)
                    .ToList();

                var auteursAvecNombreLivres = authors.Select(a => new
                {
                    Id = a.Id,
                    Nom = a.Nom,
                    Prenom = a.Prenom,
                    NombreLivres = a.Livres?.Count ?? 0
                }).ToList();

                DataGridAuthors.ItemsSource = auteursAvecNombreLivres;
            }
        }

        private void DataG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var authorselected = DataGridAuthors.SelectedItem as Auteur;
            if(authorselected == null)
            {
                return;
            }

            TxtAuthorLastName.Text = authorselected.Nom;
            TxtAuthorFirstName.Text = authorselected.Prenom;

        }
        private void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            // Votre logique existante
        }

        private void BtnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            // Votre logique existante
        }

        private void BtnDeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            // Votre logique existante
        }
        public void UpdateUIWithResources()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            //// Onglet Auteurs
            LblAuthorsTitle.Text = resourceManager.GetString("Authors");
            LblAuthorLastName.Content = $"{resourceManager.GetString("LastName")} :";
            LblAuthorFirstName.Content = $"{resourceManager.GetString("FirstName")} :";
            BtnAddAuthor.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditAuthor.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteAuthor.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            ColAuthorLastName.Header = resourceManager.GetString("LastName");
            ColAuthorFirstName.Header = resourceManager.GetString("FirstName");
            ColAuthorBooksCount.Header = resourceManager.GetString("BooksCount");
            LblManageAuthInfo.Text = resourceManager.GetString("ManageAuthInfo");
            TxtBookDetails.Text = resourceManager.GetString("AuthorManagement");

        }
    }

    internal class Auteurs
    {
    }
}