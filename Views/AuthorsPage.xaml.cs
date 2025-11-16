using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Bibliotheque_Livre.Views
{
    public partial class AuthorsPage : UserControl
    {
        private ResourceManager resourceManager;
        private DbContextBibliotheque _context;
        private Auteur _selectedAuteur;

        public AuthorsPage()
        {
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(AuthorsPage).Assembly);
            InitializeComponent();

            // Initialiser le ResourceManager
            resourceManager = new ResourceManager(
                "Gestion_Bibliotheque_Livre.Properties.Resources",
                typeof(MainWindow).Assembly);

            // Initialiser le contexte de base de données
            _context = new DbContextBibliotheque();

            // Mettre à jour l'interface avec les ressources
            UpdateUIWithResources();
            ChargerAuthors();

            // Charger les données
            ChargerAuteurs();

            // Attacher les événements
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            // Événement de sélection dans le DataGrid
            DataGridAuthors.SelectionChanged += DataGridAuthors_SelectionChanged;
        }

        // ==================== CHARGEMENT DES DONNÉES ====================

        private void ChargerAuteurs()
        {
            try
            {
                var auteurs = _context.Auteurs
                    .Include(a => a.Livres)
                    .Select(a => new
                    {
                        a.Id,
                        a.Nom,
                        a.Prenom,
                        NombreLivres = a.Livres.Count
                    })
                    .OrderBy(a => a.Nom)
                    .ThenBy(a => a.Prenom)
                    .ToList();

                DataGridAuthors.ItemsSource = auteurs;
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
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
        // ==================== ÉVÉNEMENTS ====================

        
        private void DataGridAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridAuthors.SelectedItem == null)
            {
                ViderChamps();
                _selectedAuteur = null;
                return;
            }

            try
            {
                // Récupérer l'objet anonyme sélectionné
                dynamic selectedItem = DataGridAuthors.SelectedItem;
                int auteurId = selectedItem.Id;

                // Charger l'auteur complet depuis la base
                _selectedAuteur = _context.Auteurs.Find(auteurId);

                if (_selectedAuteur != null)
                {
                    TxtAuthorLastName.Text = _selectedAuteur.Nom;
                    TxtAuthorFirstName.Text = _selectedAuteur.Prenom;
                }

                MasquerErreur();
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        // ==================== AJOUTER UN AUTEUR ====================

        private void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur();

                // Validation des champs
                if (!ValiderChamps())
                    return;

                string nom = TxtAuthorLastName.Text.Trim();
                string prenom = TxtAuthorFirstName.Text.Trim();

                // Vérifier si l'auteur existe déjà
                var auteurExiste = _context.Auteurs
                    .FirstOrDefault(a => a.Nom == nom && a.Prenom == prenom);

                if (auteurExiste != null)
                {
                    AfficherErreur("ErrorAuthorExists");
                    return;
                }

                // Créer un nouvel auteur
                var nouvelAuteur = new Auteur
                {
                    Nom = nom,
                    Prenom = prenom
                };

                _context.Auteurs.Add(nouvelAuteur);
                _context.SaveChanges();

                MessageBox.Show(
                    resourceManager.GetString("SuccessAuthorAdded") ?? "Auteur ajouté avec succès !",
                    resourceManager.GetString("Success") ?? "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Recharger et vider
                ChargerAuteurs();
                ViderChamps();
                _selectedAuteur = null;
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        // ==================== MODIFIER UN AUTEUR ====================

        private void BtnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur();

                // Vérifier qu'un auteur est sélectionné
                if (_selectedAuteur == null)
                {
                    AfficherErreur("ErrorSelectAuthor");
                    return;
                }

                // Validation des champs
                if (!ValiderChamps())
                    return;

                string nom = TxtAuthorLastName.Text.Trim();
                string prenom = TxtAuthorFirstName.Text.Trim();

                // Vérifier si un autre auteur avec le même nom existe
                var auteurExiste = _context.Auteurs
                    .FirstOrDefault(a => a.Id != _selectedAuteur.Id && a.Nom == nom && a.Prenom == prenom);

                if (auteurExiste != null)
                {
                    AfficherErreur("ErrorAuthorExists");
                    return;
                }

                // Recharger l'auteur depuis le contexte
                var auteurToUpdate = _context.Auteurs.Find(_selectedAuteur.Id);

                if (auteurToUpdate == null)
                {
                    AfficherErreur("ErrorAuthorNotFound");
                    ChargerAuteurs();
                    ViderChamps();
                    _selectedAuteur = null;
                    return;
                }

                // Mettre à jour
                auteurToUpdate.Nom = nom;
                auteurToUpdate.Prenom = prenom;
                _context.SaveChanges();

                MessageBox.Show(
                    resourceManager.GetString("SuccessAuthorUpdated") ?? "Auteur modifié avec succès !",
                    resourceManager.GetString("Success") ?? "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Recharger et vider
                ChargerAuteurs();
                ViderChamps();
                _selectedAuteur = null;
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        // ==================== SUPPRIMER UN AUTEUR ====================

        private void BtnDeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur();

                // Vérifier qu'un auteur est sélectionné
                if (_selectedAuteur == null)
                {
                    AfficherErreur("ErrorSelectAuthor");
                    return;
                }

                // Charger l'auteur avec ses livres
                var auteurAvecLivres = _context.Auteurs
                    .Include(a => a.Livres)
                    .FirstOrDefault(a => a.Id == _selectedAuteur.Id);

                if (auteurAvecLivres == null)
                {
                    AfficherErreur("ErrorAuthorNotFound");
                    ChargerAuteurs();
                    ViderChamps();
                    _selectedAuteur = null;
                    return;
                }

                int nombreLivres = auteurAvecLivres.Livres.Count;

                // Message de confirmation
                string message;
                if (nombreLivres > 0)
                {
                    message = string.Format(
                        resourceManager.GetString("ConfirmDeleteAuthorWithBooks") ??
                        "Êtes-vous sûr de vouloir supprimer l'auteur {0} {1} ?\n\n⚠️ ATTENTION : {2} livre(s) associé(s) seront également supprimés !",
                        auteurAvecLivres.Prenom,
                        auteurAvecLivres.Nom,
                        nombreLivres);
                }
                else
                {
                    message = string.Format(
                        resourceManager.GetString("ConfirmDeleteAuthor") ??
                        "Êtes-vous sûr de vouloir supprimer l'auteur {0} {1} ?",
                        auteurAvecLivres.Prenom,
                        auteurAvecLivres.Nom);
                }

                var result = MessageBox.Show(
                    message,
                    resourceManager.GetString("Confirmation") ?? "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Auteurs.Remove(auteurAvecLivres);
                    _context.SaveChanges();

                    MessageBox.Show(
                        resourceManager.GetString("SuccessAuthorDeleted") ?? "Auteur supprimé avec succès !",
                        resourceManager.GetString("Success") ?? "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Recharger et vider
                    ChargerAuteurs();
                    ViderChamps();
                    _selectedAuteur = null;
                }
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        // ==================== VALIDATION ====================

        
        private bool ValiderChamps()
        {
            if (string.IsNullOrWhiteSpace(TxtAuthorLastName.Text))
            {
                AfficherErreur("ErrorLastNameRequired");
                TxtAuthorLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtAuthorFirstName.Text))
            {
                AfficherErreur("ErrorFirstNameRequired");
                TxtAuthorFirstName.Focus();
                return false;
            }

            return true;
        }

        // ==================== GESTION DES ERREURS ====================

        private void AfficherErreur(string resourceKey, string additionalParams = null)
        {
            string message = resourceManager.GetString(resourceKey) ?? resourceKey;

            if (!string.IsNullOrEmpty(additionalParams))
            {
                message += $" : {additionalParams}";
            }

            TxtErrorMessage.Text = message;
            ErrorMessageBorder.Visibility = Visibility.Visible;

            // Timer pour masquer après 5 secondes
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += (s, e) =>
            {
                MasquerErreur();
                timer.Stop();
            };
            timer.Start();
        }

       
        private void MasquerErreur()
        {
            ErrorMessageBorder.Visibility = Visibility.Collapsed;
            TxtErrorMessage.Text = string.Empty;
        }

        // ==================== MÉTHODES UTILITAIRES ====================

       
        private void ViderChamps()
        {
            TxtAuthorLastName.Clear();
            TxtAuthorFirstName.Clear();
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

        public void ActualiserDonnees()
        {
            ChargerAuteurs();
        }

       
        public void ChangerLangue()
        {
            UpdateUIWithResources();
            ChargerAuteurs();
        }

        //supprimer un auteur
        public void SupprimerAuteur(int auteurId)
        {
            try
            {
                var auteur = _context.Auteurs
                    .Include(a => a.Livres)
                    .FirstOrDefault(a => a.Id == auteurId);
                if (auteur == null)
                {
                    AfficherErreur("ErrorAuthorNotFound");
                    return;
                }
                _context.Auteurs.Remove(auteur);
                _context.SaveChanges();
                ChargerAuteurs();
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }





        public void Cleanup()
        {
            _context?.Dispose();
        }
    }

    internal class Auteurs
    {
    }
}
}
    
