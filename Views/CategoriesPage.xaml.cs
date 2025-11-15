using BibliothequeApp.Models; // ou le bon namespace pour DbContextBibliotheque
using Gestion_Bibliotheque_Livre.Models; // ou le bon namespace pour Categorie
using Microsoft.EntityFrameworkCore;
using System.Resources;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Gestion_Bibliotheque_Livre.Views
{
    public partial class CategoriesPage : UserControl
    {
        // Gestionnaire de ressources pour les textes localisés
        private ResourceManager resourceManager;

        // Variables pour la gestion des erreurs localisées
        private string? currentErrorKey;
        private string? currentErrorParams;

        public CategoriesPage()
        {
            // Initialise le gestionnaire de ressources pour charger les chaînes localisées
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(CategoriesPage).Assembly);
            InitializeComponent();
            UpdateUIWithResources();
            // Charger les données des catégories au chargement de la page
            ChargerCategories();
        }
        public void UpdateUIWithResources()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            TxtCategoriesHeader.Text = resourceManager.GetString("Categories");
            LabelCategoryName.Content = $"{resourceManager.GetString("CategoryName")} :";
            BtnAddCategory.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditCategory.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteCategory.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            ColCategoryName.Header = resourceManager.GetString("CategoryName");
            ColCategoryBooksCount.Header = resourceManager.GetString("BooksCount");
            TxtManageBookCategories.Text = resourceManager.GetString("ManageBookCategorie");
            TxtBookDetails.Text = resourceManager.GetString("CategoryManagement");

        }

        public void ApplyLanguage()
        {
            UpdateUIWithResources();
            ChargerCategories();
            RafraichirMessageErreur();
        }

        /// <summary>
        /// Charge la liste des catégories depuis la base et les affiche dans le DataGrid.
        /// Inclut le nombre de livres par catégorie.
        /// </summary>
        private void ChargerCategories()
        {
            using (var ListeDesLivres = new DbContextBibliotheque())
            {
                var categories = ListeDesLivres.Categories
                    .OrderBy(c => c.Nom)
                    .Select(c => new
                    {
                        c.Id,
                        c.Nom,
                        NombreLivres = c.LivreCategories.Count() // Calcul du nombre de livres liés
                    })
                    .ToList();

                // Assigner au DataGrid
                DataGridCategories.ItemsSource = categories;
            }
        }

        /// <summary>
        /// Gère la sélection d’une catégorie dans le DataGrid.
        /// Remplit le champ de texte avec le nom de la catégorie sélectionnée.
        /// </summary>
        private void DataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
            if (element == null)
            {
                TxtCategoryName.Clear(); // Désélectionne le texte si aucune catégorie n'est sélectionnée
                return;
            }

            // Les éléments sont des objets anonymes { Id, Nom, NombreLivres } → on lit via réflexion
            var nom = element.GetType().GetProperty("Nom")?.GetValue(element)?.ToString();
            TxtCategoryName.Text = nom ?? string.Empty;
        }

        /// <summary>
        /// Gestionnaire du bouton "Ajouter une catégorie".
        /// Valide le nom, vérifie les doublons, ajoute la catégorie à la base.
        /// Affiche un message de succès ou d’erreur.
        /// </summary>
        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur(); // Cache tout message d'erreur précédent

                // Valide le champ du nom
                if (!ValiderFormulaireCategorie())
                    return;

                var nom = TxtCategoryName.Text?.Trim();

                using (var ListeDesLivres = new DbContextBibliotheque())
                {
                    // Vérifie si une catégorie avec le même nom existe déjà (insensible à la casse)
                    if (ListeDesLivres.Categories.Any(c => c.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase)))
                    {
                        AfficherErreur("ErrorCategoryExists"); // Utilisez une clé de ressource appropriée
                        return;
                    }

                    // Crée et ajoute la nouvelle catégorie
                    ListeDesLivres.Categories.Add(new Categorie { Nom = nom });
                    ListeDesLivres.SaveChanges();
                }

                // Vide le champ de texte après ajout
                TxtCategoryName.Clear();

                // Recharge la liste des catégories pour afficher la nouvelle
                ChargerCategories();

                // Affiche un message de succès
                MessageBox.Show(
                    resourceManager.GetString("SuccessCategoryAdded") ?? "La catégorie a été ajoutée avec succès !", // Utilisez une clé de ressource appropriée
                    resourceManager.GetString("Success") ?? "Succès", // Utilisez une clé de ressource appropriée
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                // En cas d'erreur inattendue, affiche le message avec le détail de l'exception
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        /// <summary>
        /// Gestionnaire du bouton "Modifier une catégorie".
        /// Valide le nom, vérifie les doublons, met à jour la catégorie sélectionnée dans la base.
        /// Affiche un message de succès ou d’erreur.
        /// </summary>
        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur(); // Cache tout message d'erreur précédent

                // Valide le champ du nom
                if (!ValiderFormulaireCategorie())
                    return;

                var element = DataGridCategories.SelectedItem;
                if (element == null)
                {
                    AfficherErreur("ErrorCategorySelect"); // Utilisez une clé de ressource appropriée
                    return;
                }

                // Récupère l'ID de la catégorie sélectionnée (depuis l'objet anonyme)
                var type = element.GetType();
                var idObj = type.GetProperty("Id")?.GetValue(element);
                if (idObj is not int id)
                {
                    AfficherErreur("ErrorInvalidSelection"); // Utilisez une clé de ressource appropriée
                    return;
                }

                var nouveauNom = TxtCategoryName.Text?.Trim();

                using (var ListeDesLivres = new DbContextBibliotheque())
                {
                    // Vérifie si une autre catégorie porte déjà ce nom (insensible à la casse)
                    if (ListeDesLivres.Categories.Any(c => c.Nom.Equals(nouveauNom, StringComparison.OrdinalIgnoreCase) && c.Id != id))
                    {
                        AfficherErreur("ErrorCategoryExists"); // Utilisez une clé de ressource appropriée
                        return;
                    }

                    // Trouve la catégorie dans la base (pas l'objet anonyme du DataGrid)
                    var categorie = ListeDesLivres.Categories.Find(id);
                    if (categorie == null)
                    {
                        // La catégorie a peut-être été supprimée par ailleurs, recharge la liste
                        ChargerCategories();
                        TxtCategoryName.Clear();
                        AfficherErreur("ErrorCategoryNotFound"); // Utilisez une clé de ressource appropriée
                        return;
                    }

                    // Met à jour le nom
                    categorie.Nom = nouveauNom;
                    ListeDesLivres.SaveChanges();
                }

                // Recharge la liste des catégories pour refléter les modifications
                ChargerCategories();

                // Affiche un message de succès
                MessageBox.Show(
                    resourceManager.GetString("SuccessCategoryUpdated") ?? "La catégorie a été mise à jour avec succès !", // Utilisez une clé de ressource appropriée
                    resourceManager.GetString("Success") ?? "Succès", // Utilisez une clé de ressource appropriée
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                // En cas d'erreur inattendue, affiche le message avec le détail de l'exception
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        /// <summary>
        /// Gestionnaire du bouton "Supprimer une catégorie".
        /// Supprime la catégorie sélectionnée de la base.
        /// Affiche un message de succès ou d’erreur.
        /// </summary>
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var element = DataGridCategories.SelectedItem;
                if (element == null)
                {
                    AfficherErreur("ErrorCategorySelect"); // Utilisez une clé de ressource appropriée
                    return;
                }

                var type = element.GetType();
                var idObj = type.GetProperty("Id")?.GetValue(element);
                var nom = type.GetProperty("Nom")?.GetValue(element)?.ToString();
                if (idObj is not int id)
                {
                    AfficherErreur("ErrorInvalidSelection"); // Utilisez une clé de ressource appropriée
                    return;
                }

                // Demande confirmation avant suppression
                var result = MessageBox.Show(
                    string.Format(resourceManager.GetString("ConfirmDeleteCategory") ?? "Supprimer la catégorie '{0}' ?", nom), // Utilisez une clé de ressource appropriée
                    resourceManager.GetString("Confirm") ?? "Confirmation", // Utilisez une clé de ressource appropriée
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result != MessageBoxResult.Yes)
                    return;

                using (var ListeDesLivres = new DbContextBibliotheque())
                {
                    var categorie = ListeDesLivres.Categories.Find(id);
                    if (categorie == null)
                    {
                        // La catégorie a peut-être été supprimée par ailleurs, recharge la liste
                        ChargerCategories();
                        TxtCategoryName.Clear();
                        AfficherErreur("ErrorCategoryNotFound"); // Utilisez une clé de ressource appropriée
                        return;
                    }

                    // Suppression de la catégorie
                    ListeDesLivres.Categories.Remove(categorie);
                    ListeDesLivres.SaveChanges();
                }

                // Vide le champ de texte après suppression
                TxtCategoryName.Clear();

                // Recharge la liste des catégories pour refléter la suppression
                ChargerCategories();

                // Affiche un message de succès
                MessageBox.Show(
                    resourceManager.GetString("SuccessCategoryDeleted") ?? "La catégorie a été supprimée avec succès !", // Utilisez une clé de ressource appropriée
                    resourceManager.GetString("Success") ?? "Succès", // Utilisez une clé de ressource appropriée
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                // En cas d'erreur inattendue, affiche le message avec le détail de l'exception
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        /// <summary>
        /// Valide le champ de saisie du nom de la catégorie.
        /// Affiche un message d'erreur si le champ est vide.
        /// Retourne true si le champ est valide, false sinon.
        /// </summary>
        private bool ValiderFormulaireCategorie()
        {
            if (string.IsNullOrWhiteSpace(TxtCategoryName.Text))
            {
                AfficherErreur("ErrorCategoryNameRequired"); // Utilisez une clé de ressource appropriée
                return false;
            }
            return true; // Le champ est valide
        }

        /// <summary>
        /// Affiche un message d'erreur dans l'interface de la page.
        /// Stocke la clé de ressource et un éventuel paramètre pour permettre la traduction dynamique.
        /// Affiche le message et le masque automatiquement après 5 secondes.
        /// </summary>
        private void AfficherErreur(string resourceKey, string? additionalParams = null)
        {
            currentErrorKey = resourceKey;
            currentErrorParams = additionalParams;

            RafraichirMessageErreur(); // Met à jour le texte affiché
            ErrorMessageBorder.Visibility = Visibility.Visible;

            // Timer pour masquer le message après 5 secondes
            var timer = new DispatcherTimer
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
        /// Rafraîchit le texte du message d'erreur en fonction de la langue courante.
        /// Utilise la clé stockée (currentErrorKey) pour récupérer la traduction.
        /// Ajoute un paramètre si présent (ex: message d'exception).
        /// </summary>
        private void RafraichirMessageErreur()
        {
            if (string.IsNullOrEmpty(currentErrorKey))
                return;

            string message = resourceManager.GetString(currentErrorKey) ?? currentErrorKey;

            if (!string.IsNullOrEmpty(currentErrorParams))
            {
                message += $" : {currentErrorParams}";
            }

            TxtErrorMessage.Text = message;
        }

        /// <summary>
        /// Masque le message d'erreur en réinitialisant l'affichage.
        /// </summary>
        private void MasquerErreur()
        {
            ErrorMessageBorder.Visibility = Visibility.Collapsed;
            TxtErrorMessage.Text = string.Empty;
            // La clé reste stockée pour être rafraîchie si la langue change
        }
    }
}