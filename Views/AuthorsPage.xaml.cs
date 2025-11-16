using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace Gestion_Bibliotheque_Livre.Views
{
    /// <summary>
    /// Logique de interaction pour la page de gestion des auteurs.
    /// Permet d'afficher, ajouter, modifier et supprimer des auteurs via une interface utilisateur.
    /// </summary>
    public partial class AuthorsPage : UserControl
    {
        #region Champs privés

        /// <summary>
        /// Gestionnaire de ressources pour la gestion de l'internationalisation (français/anglais).
        /// </summary>
        private ResourceManager resourceManager;

        #endregion

        #region Constructeur

        /// <summary>
        /// Initialise une nouvelle instance de la page AuthorsPage.
        /// Configure le ResourceManager, charge les données, met à jour l'interface et attache les événements.
        /// </summary>
        public AuthorsPage()
        {
            // Initialisation du ResourceManager pour les ressources localisées
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(AuthorsPage).Assembly);
            InitializeComponent();

            // Mise à jour de l'interface avec les ressources localisées
            UpdateUIWithResources();

            // Chargement initial des auteurs dans le DataGrid
            ChargerAuteurs();

            // Attache les gestionnaires d'événements nécessaires
            AttachEventHandlers();
        }

        #endregion

        #region Gestion des événements UI

        /// <summary>
        /// Attache les gestionnaires d'événements dynamiques (hors XAML).
        /// </summary>
        private void AttachEventHandlers()
        {
            // Attache l'événement de changement de sélection du DataGrid
            DataGridAuthors.SelectionChanged += DataGridAuthors_SelectionChanged;
        }

        #endregion

        #region Chargement des données

        /// <summary>
        /// Charge la liste des auteurs depuis la base de données et la lie au DataGrid.
        /// Inclut le nombre de livres associés à chaque auteur.
        /// </summary>
        private void ChargerAuteurs()
        {
            using var context = new DbContextBibliotheque();
            try
            {
                // Récupération des auteurs avec leurs livres et calcul du nombre de livres
                var auteurs = context.Auteurs
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

                // Liaison des données au DataGrid
                DataGridAuthors.ItemsSource = auteurs;
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors du chargement des données
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        #endregion

        #region Gestion des événements de sélection

        /// <summary>
        /// Gestionnaire d'événement pour le changement de sélection dans le DataGrid.
        /// Met à jour les champs de texte avec les valeurs de l'auteur sélectionné.
        /// </summary>
        /// <param name="sender">La source de l'événement.</param>
        /// <param name="e">Détails de l'événement de sélection.</param>
        private void DataGridAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridAuthors.SelectedItem == null)
            {
                // Désélection : vider les champs
                ViderChamps();
                return;
            }

            try
            {
                // Récupération de l'objet anonyme sélectionné dans le DataGrid
                var selectedItem = DataGridAuthors.SelectedItem;
                if (selectedItem != null)
                {
                    // Accès aux propriétés de l'objet anonyme via la réflexion
                    var nomProperty = selectedItem.GetType().GetProperty("Nom");
                    var prenomProperty = selectedItem.GetType().GetProperty("Prenom");

                    if (nomProperty != null && prenomProperty != null)
                    {
                        // Mise à jour des champs de texte
                        TxtAuthorLastName.Text = nomProperty.GetValue(selectedItem)?.ToString();
                        TxtAuthorFirstName.Text = prenomProperty.GetValue(selectedItem)?.ToString();
                    }
                }

                // Masquer les messages d'erreur précédents
                MasquerErreur();
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors de la sélection
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        #endregion

        #region Actions CRUD

        /// <summary>
        /// Gestionnaire d'événement pour le bouton d'ajout d'un auteur.
        /// Valide les champs, vérifie les doublons, sauvegarde en base et recharge la liste.
        /// Affiche un message de succès ou d'erreur dans l'interface.
        /// </summary>
        /// <param name="sender">La source de l'événement.</param>
        /// <param name="e">Détails de l'événement du clic.</param>
        private void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            using var context = new DbContextBibliotheque();
            try
            {
                // Masquer les messages d'erreurs précédents
                MasquerErreur();

                // Validation des champs obligatoires
                if (!ValiderChamps())
                    return;

                string nom = TxtAuthorLastName.Text.Trim();
                string prenom = TxtAuthorFirstName.Text.Trim();

                // Vérification de l'existence d'un auteur avec le même nom/prénom
                var auteurExiste = context.Auteurs
                    .FirstOrDefault(a => string.Equals(a.Nom, nom, System.StringComparison.OrdinalIgnoreCase)
                                      && string.Equals(a.Prenom, prenom, System.StringComparison.OrdinalIgnoreCase));

                if (auteurExiste != null)
                {
                    // Affichage d'une erreur si l'auteur existe déjà
                    AfficherErreur("ErrorAuthorExists");
                    return;
                }

                // Création et ajout du nouvel auteur
                var nouvelAuteur = new Auteur
                {
                    Nom = nom,
                    Prenom = prenom
                };

                context.Auteurs.Add(nouvelAuteur);
                context.SaveChanges(); // Sauvegarde en base de données

                // Affichage d'un message de succès dans l'interface
                TxtErrorMessage.Text = resourceManager.GetString("SuccessAuthorAdded") ?? "Auteur ajouté avec succès !";
                ErrorMessageBorder.Visibility = Visibility.Visible;
                // Vous pouvez définir un style spécifique pour les messages de succès ici si disponible
                // ErrorMessageBorder.Style = (Style)FindResource("SuccessMessageBorderStyle");

                // Recharger la liste des auteurs et vider les champs de saisie
                ChargerAuteurs();
                ViderChamps();

                // Masquer le message de succès après 3 secondes
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                timer.Tick += (s, args) =>
                {
                    MasquerErreur();
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors de l'ajout
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        /// <summary>
        /// Gestionnaire d'événement pour le bouton de modification d'un auteur.
        /// Récupère l'auteur sélectionné, met à jour ses données et sauvegarde en base.
        /// </summary>
        /// <param name="sender">La source de l'événement.</param>
        /// <param name="e">Détails de l'événement du clic.</param>
        private void BtnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            using var context = new DbContextBibliotheque();
            try
            {
                // Masquer les messages d'erreurs précédents
                MasquerErreur();

                // Récupération de l'ID à partir de l'objet anonyme sélectionné
                var selectedItem = DataGridAuthors.SelectedItem;
                if (selectedItem == null)
                {
                    AfficherErreur("ErrorSelectAuthor");
                    return;
                }

                var idProperty = selectedItem.GetType().GetProperty("Id");
                if (idProperty == null || !(idProperty.GetValue(selectedItem) is int auteurId))
                {
                    AfficherErreur("ErrorUnexpected", "L'ID de l'auteur n'est pas valide.");
                    return;
                }

                // Validation des champs obligatoires
                if (!ValiderChamps())
                    return;

                string nom = TxtAuthorLastName.Text.Trim();
                string prenom = TxtAuthorFirstName.Text.Trim();

                // Vérification de l'existence d'un autre auteur avec le même nom/prénom (hors auteur modifié)
                var auteurExiste = context.Auteurs
                    .FirstOrDefault(a => a.Id != auteurId && a.Nom == nom && a.Prenom == prenom);

                if (auteurExiste != null)
                {
                    AfficherErreur("ErrorAuthorExists");
                    return;
                }

                // Chargement de l'auteur à modifier depuis la base
                var auteurToUpdate = context.Auteurs.Find(auteurId);

                if (auteurToUpdate == null)
                {
                    AfficherErreur("ErrorAuthorNotFound");
                    ChargerAuteurs();
                    ViderChamps();
                    return;
                }

                // Mise à jour des propriétés de l'auteur
                auteurToUpdate.Nom = nom;
                auteurToUpdate.Prenom = prenom;
                context.SaveChanges(); // Sauvegarde en base

                // Affichage d'un message de succès
                TxtErrorMessage.Text = resourceManager.GetString("SuccessAuthorUpdated") ?? "Auteur modifié avec succès !";
                ErrorMessageBorder.Visibility = Visibility.Visible;
                // Vous pouvez définir un style spécifique pour les messages de succès ici si disponible
                // ErrorMessageBorder.Style = (Style)FindResource("SuccessMessageBorderStyle");

                // Recharger la liste et vider les champs
                ChargerAuteurs();
                ViderChamps();

                // Masquer le message après 3 secondes
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                timer.Tick += (s, args) =>
                {
                    MasquerErreur();
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors de la modification
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        /// <summary>
        /// Gestionnaire d'événement pour le bouton de suppression d'un auteur.
        /// Affiche un message de confirmation, supprime l'auteur et ses livres, recharge la liste.
        /// </summary>
        /// <param name="sender">La source de l'événement.</param>
        /// <param name="e">Détails de l'événement du clic.</param>
        private void BtnDeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridAuthors.SelectedItem == null)
            {
                // Aucun auteur sélectionné
                AfficherErreur("ErrorSelectAuthor");
                return;
            }

            try
            {
                // Masquer les messages d'erreurs précédents
                MasquerErreur();

                // Récupération de l'ID à partir de l'objet anonyme sélectionné
                var selectedItem = DataGridAuthors.SelectedItem;
                if (selectedItem == null)
                {
                    AfficherErreur("ErrorSelectAuthor");
                    return;
                }

                var idProperty = selectedItem.GetType().GetProperty("Id");
                if (idProperty == null)
                {
                    AfficherErreur("ErrorUnexpected", "Impossible de récupérer l'ID de l'auteur.");
                    return;
                }

                if (!(idProperty.GetValue(selectedItem) is int auteurId))
                {
                    AfficherErreur("ErrorUnexpected", "L'ID de l'auteur n'est pas valide.");
                    return;
                }

                // Charger l'auteur complet depuis la base avec ses livres
                using var context = new DbContextBibliotheque();
                var auteurAvecLivres = context.Auteurs
                    .Include(a => a.Livres)
                    .FirstOrDefault(a => a.Id == auteurId);

                if (auteurAvecLivres == null)
                {
                    AfficherErreur("ErrorAuthorNotFound");
                    ChargerAuteurs();
                    ViderChamps();
                    return;
                }

                // Calcul du nombre de livres associés
                int nombreLivres = auteurAvecLivres.Livres.Count;

                // Suppression de l'auteur et de ses livres de la base
                context.Auteurs.Remove(auteurAvecLivres);
                context.SaveChanges();

                // Affichage d'un message de succès
                TxtErrorMessage.Text = resourceManager.GetString("SuccessAuthorDeleted") ?? "Auteur supprimé avec succès !";
                ErrorMessageBorder.Visibility = Visibility.Visible;
                // Vous pouvez définir un style spécifique pour les messages de succès ici si disponible
                // ErrorMessageBorder.Style = (Style)FindResource("SuccessMessageBorderStyle");

                // Recharger la liste et vider les champs
                ChargerAuteurs();
                ViderChamps();

                // Masquer le message après 3 secondes
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                timer.Tick += (s, args) =>
                {
                    MasquerErreur();
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors de la suppression
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        #endregion

        #region Validation des champs

        /// <summary>
        /// Valide les champs de saisie (nom et prénom).
        /// Affiche une erreur et met le focus sur le premier champ invalide.
        /// </summary>
        /// <returns>True si les champs sont valides, false sinon.</returns>
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

        #endregion

        #region Gestion des erreurs

        /// <summary>
        /// Affiche un message d'erreur dans l'interface utilisateur.
        /// Le message est récupéré à partir des ressources localisées.
        /// Un timer est lancé pour masquer automatiquement le message après 5 secondes.
        /// </summary>
        /// <param name="resourceKey">Clé du message dans les ressources.</param>
        /// <param name="additionalParams">Paramètres supplémentaires à ajouter au message.</param>
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

        /// <summary>
        /// Masque le message d'erreur actuel dans l'interface.
        /// </summary>
        private void MasquerErreur()
        {
            ErrorMessageBorder.Visibility = Visibility.Collapsed;
            TxtErrorMessage.Text = string.Empty;
        }

        #endregion

        #region Méthodes utilitaires

        /// <summary>
        /// Vide les champs de saisie (nom et prénom).
        /// </summary>
        private void ViderChamps()
        {
            TxtAuthorLastName.Clear();
            TxtAuthorFirstName.Clear();
        }

        /// <summary>
        /// Met à jour les textes de l'interface utilisateur avec les ressources localisées.
        /// </summary>
        public void UpdateUIWithResources()
        {
            // Mise à jour des textes localisés pour les contrôles de l'interface
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

        /// <summary>
        /// Actualise les données affichées dans le DataGrid.
        /// </summary>
        public void ActualiserDonnees()
        {
            ChargerAuteurs();
        }

        /// <summary>
        /// Met à jour l'interface et recharge les données suite à un changement de langue.
        /// </summary>
        public void ChangerLangue()
        {
            UpdateUIWithResources();
            ChargerAuteurs();
        }

        #endregion
    }
}