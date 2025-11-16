using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;
using System.Resources; // Nécessaire pour ResourceManager
using System.Globalization; 
using System.Windows;
using System.Windows.Controls;

namespace Gestion_Bibliotheque_Livre.Views
{
    public partial class BooksPage : UserControl
    {
        // 1. Déclaration de resourceManager en tant que variable de classe
        private ResourceManager resourceManager;

        // Stocke la clé de ressource du message d'erreur actuel (ex: "ErrorBookTitle")
        private string? currentErrorKey;

        // Stocke un paramètre additionnel éventuel pour le message d'erreur (ex: le message d'exception)
        private string? currentErrorParams;

        public BooksPage()
        {
            // Initialise le gestionnaire de ressources pour charger les chaînes localisées
            // Il est préférable de passer l'assembly de la classe courante
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(BooksPage).Assembly);

            InitializeComponent();
            // Charger les données des livres, auteurs et catégories ici
            ChargerLivres();
            UpdateUIWithResources();
            ChargerAuteurs();
            ChargerCategories(); 
            UpdateDefaultStyle();
        }

        public void UpdateUIWithResources()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            //// Onglet Livres
            TxtBooks.Text = resourceManager.GetString("Books");
            LabelBookTitle.Content = $"{resourceManager.GetString("BookTitle")} :";
            LabelAuthor.Content = $"{resourceManager.GetString("Authors")} :";
            LabelPublicationDate.Content = $"{resourceManager.GetString("Date")} :";
            LabelISBN.Content = $"{resourceManager.GetString("ISBN")} :";
            LabelPages.Content = $"{resourceManager.GetString("NumberOfPages")} :";
            LabelCategoriesBook.Content = $"{resourceManager.GetString("Categories")} :";
            LabelCopies.Content = $"{resourceManager.GetString("NumberOfCopies")} :";
            BtnAddBook.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditBook.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteBook.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            ColBookTitle.Header = resourceManager.GetString("BookTitle");
            ColBookAuthor.Header = resourceManager.GetString("Author");
            ColBookDate.Header = resourceManager.GetString("Date");
            ColBookISBN.Header = resourceManager.GetString("ISBN");
            ColBookPages.Header = resourceManager.GetString("Pages");
            ColBookCopies.Header = resourceManager.GetString("Copies");
            TxtManageBookDetails.Text = resourceManager.GetString("ManageBookDetails");
            TxtBookDetails.Text = resourceManager.GetString("BookManagement");
            DatePickerPublication.Text = resourceManager.GetString("DatePicker_Placeholder");

        }

        public void ApplyLanguage()
        {
            UpdateUIWithResources();

            ChargerAuteurs();
            ChargerCategories();
            ChargerLivres();
            RafraichirMessageErreur();
        }

        /// <summary>
        /// Charge la liste des livres depuis la base de données.
        /// Inclut les relations : Auteur, DetailsLivre et LivreCategories → Categorie.
        /// Recrée les objets Livre pour forcer le recalcul de DatePublicationFormatee selon la langue.
        /// </summary>
        public void ChargerLivres()
        {
            using (var ListeDesLivres = new DbContextBibliotheque())
            {
                var livresFromDb = ListeDesLivres.Livres
                    .Include(l => l.Auteur)
                    .Include(l => l.DetailsLivre)
                    .Include(l => l.LivreCategories)
                        .ThenInclude(lc => lc.Categorie)
                    .ToList();

                // Recrée les objets Livre pour forcer le recalcul de DatePublicationFormatee
                var livresWithUpdatedCulture = livresFromDb.Select(l => new Livre
                {
                    Id = l.Id,
                    Titre = l.Titre,
                    DatePublication = l.DatePublication,
                    AuteurId = l.AuteurId,
                    Auteur = l.Auteur,
                    DetailsLivre = l.DetailsLivre,
                    LivreCategories = l.LivreCategories
                }).ToList();

                dgvBooks.ItemsSource = livresWithUpdatedCulture;
            }
        }

        /// <summary>
        /// Charge la liste des auteurs depuis la base et les affiche dans le ComboBoxAuthorSelect.
        /// Ajoute une option par défaut "Sélectionner un auteur".
        /// </summary>
        public void ChargerAuteurs()
        {
            using (var ListeDesLivres = new DbContextBibliotheque())
            {
                var auteurs = ListeDesLivres.Auteurs
                    .OrderBy(a => a.Nom)
                    .ThenBy(a => a.Prenom)
                    .ToList();

                ComboBoxAuthorSelect.Items.Clear();

                ComboBoxAuthorSelect.Items.Add(new ComboBoxItem
                {
                    Content = resourceManager.GetString("SelectAuthor"), 
                    Tag = null
                });

                foreach (var auteur in auteurs)
                {
                    ComboBoxAuthorSelect.Items.Add(new ComboBoxItem
                    {
                        Content = auteur.NomComplet,
                        Tag = auteur.Id
                    });
                }

                ComboBoxAuthorSelect.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Charge la liste des catégories depuis la base et les affiche dans le ComboBoxCategoriesSelect.
        /// Ajoute une option par défaut "Sélectionner une catégorie".
        /// </summary>
        public void ChargerCategories()
        {
            using (var DonneeDB = new DbContextBibliotheque())
            {
                var categories = DonneeDB.Categories
                    .OrderBy(c => c.Nom)
                    .Select(c => new Categorie
                    {
                        Id = c.Id,
                        Nom = c.Nom
                    })
                    .ToList();

                ListBoxCategoriesSelect.ItemsSource = categories;
                ListBoxCategoriesSelect.SelectedValuePath = "Id";
                ListBoxCategoriesSelect.DisplayMemberPath = "Nom";
            }
        }

        // Récupère les IDs des catégories sélectionnées
        private List<int> GetSelectedCategoryIds()
        {
            return ListBoxCategoriesSelect.SelectedItems
                .Cast<Categorie>()
                .Select(c => c.Id)
                .ToList();
        }

        /// <summary>
        /// Gère la sélection d’un livre dans le DataGrid.
        /// Remplit les champs du formulaire avec les données du livre sélectionné.
        /// Sélectionne automatiquement l’auteur et la catégorie dans les ComboBox.
        /// </summary>
        private void dgvBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedLivre = dgvBooks.SelectedItem as Livre;
            if (selectedLivre == null)
                return;

            TxtBookTitle.Text = selectedLivre.Titre;
            TxtISBN.Text = selectedLivre.DetailsLivre?.ISBN;
            TxtPages.Text = selectedLivre.DetailsLivre?.NombrePages.ToString();
            TxtCopies.Text = selectedLivre.DetailsLivre?.NombreExemplaires.ToString();
            DatePickerPublication.SelectedDate = selectedLivre.DatePublication;

            foreach (ComboBoxItem item in ComboBoxAuthorSelect.Items)
            {
                if (item.Tag is int auteurId && auteurId == selectedLivre.AuteurId)
                {
                    ComboBoxAuthorSelect.SelectedItem = item;
                    break;
                }
            }

            var bookCategoryIds = selectedLivre.LivreCategories.Select(lc => lc.CategorieId).ToHashSet();
            ListBoxCategoriesSelect.SelectedItems.Clear();
            foreach (var cat in ListBoxCategoriesSelect.Items.Cast<Categorie>())
            {
                if (bookCategoryIds.Contains(cat.Id))
                    ListBoxCategoriesSelect.SelectedItems.Add(cat);
            }
        }

        /// <summary>
        /// Gestionnaire du bouton "Ajouter un livre".
        /// Valide les champs, vérifie les doublons, ajoute le livre et sa catégorie à la base.
        /// Affiche un message de succès ou d’erreur.
        /// </summary>
        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur();
                if (!ValiderFormulaireLivre())
                    return;

                if (!(ComboBoxAuthorSelect.SelectedItem is ComboBoxItem authorItem) || !(authorItem.Tag is int auteurId))
                {
                    AfficherErreur("ErrorAuthorRequired");
                    return;
                }

                var selectedCategoryIds = GetSelectedCategoryIds();

                using (var ctx = new DbContextBibliotheque())
                {
                    // Doublon titre+auteur
                    if (ctx.Livres.Any(l => l.Titre == TxtBookTitle.Text.Trim() && l.AuteurId == auteurId))
                    {
                        AfficherErreur("ErrorBookExists");
                        return;
                    }

                    var livre = new Livre
                    {
                        Titre = TxtBookTitle.Text.Trim(),
                        DatePublication = DatePickerPublication.SelectedDate ?? DateTime.Now,
                        AuteurId = auteurId,
                        DetailsLivre = new DetailsLivre
                        {
                            ISBN = TxtISBN.Text,
                            NombrePages = int.TryParse(TxtPages.Text, out int pages) ? pages : 0,
                            NombreExemplaires = int.TryParse(TxtCopies.Text, out int copies) ? copies : 0
                        },
                        LivreCategories = new List<LivreCategorie>()
                    };

                    ctx.Livres.Add(livre);
                    ctx.SaveChanges();

                    foreach (var catId in selectedCategoryIds)
                    {
                        ctx.LivreCategories.Add(new LivreCategorie
                        {
                            LivreId = livre.Id,
                            CategorieId = catId
                        });
                    }
                    ctx.SaveChanges();
                }

                ViderFormulaireLivre();
                ChargerLivres();
                MessageBox.Show(resourceManager.GetString("SuccessBookAdded") ?? "Livre ajouté.",
                    resourceManager.GetString("Success") ?? "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }
        /// <summary>
        /// Valide tous les champs du formulaire d'ajout de livre.
        /// Affiche un message d'erreur si un champ est invalide.
        /// Retourne true si tout est valide, false sinon.
        /// </summary>
               // Validation: au moins une catégorie
        private bool ValiderFormulaireLivre()
        {
            if (string.IsNullOrWhiteSpace(TxtBookTitle.Text))
            {
                AfficherErreur("ErrorBookTitle");
                return false;
            }
            if (ComboBoxAuthorSelect.SelectedIndex <= 0)
            {
                AfficherErreur("ErrorAuthorRequired");
                return false;
            }
            if (DatePickerPublication.SelectedDate == null)
            {
                AfficherErreur("ErrorDateRequired");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtISBN.Text))
            {
                AfficherErreur("ErrorISBNRequired");
                return false;
            }
            if (!int.TryParse(TxtPages.Text, out int pages) || pages <= 0)
            {
                AfficherErreur("ErrorPagesInvalid");
                return false;
            }
            if (!int.TryParse(TxtCopies.Text, out int copies) || copies < 0)
            {
                AfficherErreur("ErrorCopiesInvalid");
                return false;
            }
            if (GetSelectedCategoryIds().Count == 0)
            {
                AfficherErreur("ErrorCategoryRequired");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Affiche un message d'erreur dans l'interface.
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
        /// Rafraîchit le texte du message d'erreur en fonction de la langue courante.
        /// Utilise la clé stockée (currentErrorKey) pour récupérer la traduction.
        /// Ajoute un paramètre si présent (ex: message d'exception).
        /// </summary>
        private void RafraichirMessageErreur()
        {
            if (string.IsNullOrEmpty(currentErrorKey))
                return;

            string message = resourceManager.GetString(currentErrorKey) ?? currentErrorKey; // Utilisation de resourceManager

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

        /// <summary>
        /// Vide tous les champs du formulaire d'ajout de livre.
        /// Réinitialise les sélections dans les ComboBox.
        /// </summary>
        private void ViderFormulaireLivre()
        {
            TxtBookTitle.Clear();
            TxtISBN.Clear();
            TxtPages.Clear();
            TxtCopies.Clear();
            DatePickerPublication.SelectedDate = null;
            ComboBoxAuthorSelect.SelectedIndex = 0;
            ListBoxCategoriesSelect.SelectedItems.Clear();
        }

        /// <summary>
        /// Gestionnaire du bouton "Supprimer un livre".
        /// Supprime le livre sélectionné ainsi que ses relations et détails.
        /// </summary>
        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedLivre = dgvBooks.SelectedItem as Livre;

                if (selectedLivre == null)
                {
                    AfficherErreur("ErrorBookTitle");
                    return;
                }

                using (var ListeDesLivres = new DbContextBibliotheque())
                {
                    var livreToDelete = ListeDesLivres.Livres
                        .Include(l => l.DetailsLivre)
                        .Include(l => l.LivreCategories)
                        .FirstOrDefault(l => l.Id == selectedLivre.Id);

                    if (livreToDelete == null)
                    {
                        ChargerLivres(); // 3. Correction : appel de ChargerLivres (pluriel)
                        ViderFormulaireLivre();
                        return;
                    }

                    // Supprimer les relations LivreCategorie associées
                    if (livreToDelete.LivreCategories != null && livreToDelete.LivreCategories.Count > 0)
                    {
                        ListeDesLivres.LivreCategories.RemoveRange(livreToDelete.LivreCategories);
                    }

                    // Supprimer les détails du livre associés
                    if (livreToDelete.DetailsLivre != null)
                    {
                        ListeDesLivres.DetailsLivres.Remove(livreToDelete.DetailsLivre);
                    }

                    ListeDesLivres.Livres.Remove(livreToDelete);
                    ListeDesLivres.SaveChanges();
                }
                // Recharge la liste des livres et vide le formulaire
                ChargerLivres(); // 3. Correction : appel de ChargerLivres (pluriel)
                ViderFormulaireLivre();
            }
            catch (Exception ex)
            {
                // En cas d'erreur inattendue, affiche le message avec le détail de l'exception
                AfficherErreur("ErrorUnexpected", ex.Message);
                return;
            }
        }

        /// <summary>
        /// Gestionnaire du bouton "Modifier un livre".
        /// Met à jour le livre sélectionné avec les données du formulaire.
        /// </summary>
        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MasquerErreur();
                if (!ValiderFormulaireLivre())
                    return;

                if (!(ComboBoxAuthorSelect.SelectedItem is ComboBoxItem authorItem) || !(authorItem.Tag is int auteurId))
                {
                    AfficherErreur("ErrorAuthorRequired");
                    return;
                }

                var selectedLivre = dgvBooks.SelectedItem as Livre;
                if (selectedLivre == null)
                {
                    AfficherErreur("ErrorBookTitle");
                    return;
                }

                var selectedCategoryIds = GetSelectedCategoryIds();

                using (var donneDB = new DbContextBibliotheque())
                {
                    var livreToUpdate = donneDB.Livres
                        .Include(l => l.DetailsLivre)
                        .Include(l => l.LivreCategories)
                        .FirstOrDefault(l => l.Id == selectedLivre.Id);

                    if (livreToUpdate == null)
                    {
                        ChargerLivres();
                        return;
                    }

                    string nouveauTitre = TxtBookTitle.Text.Trim();
                    bool existeDeja = donneDB.Livres.Any(l => l.Id != livreToUpdate.Id && l.Titre == nouveauTitre && l.AuteurId == auteurId);
                    if (existeDeja)
                    {
                        AfficherErreur("ErrorBookExists");
                        return;
                    }

                    livreToUpdate.Titre = nouveauTitre;
                    livreToUpdate.DatePublication = DatePickerPublication.SelectedDate ?? DateTime.Now;
                    livreToUpdate.AuteurId = auteurId;

                    if (livreToUpdate.DetailsLivre == null)
                        livreToUpdate.DetailsLivre = new DetailsLivre { LivreId = livreToUpdate.Id };

                    livreToUpdate.DetailsLivre.ISBN = TxtISBN.Text;
                    livreToUpdate.DetailsLivre.NombrePages = int.TryParse(TxtPages.Text, out int pages) ? pages : 0;
                    livreToUpdate.DetailsLivre.NombreExemplaires = int.TryParse(TxtCopies.Text, out int copies) ? copies : 0;

                    // Mise à jour catégories
                    var existing = livreToUpdate.LivreCategories.Select(lc => lc.CategorieId).ToHashSet();

                    // Ajouter nouvelles
                    foreach (var catId in selectedCategoryIds.Where(id => !existing.Contains(id)))
                    {
                        livreToUpdate.LivreCategories.Add(new LivreCategorie
                        {
                            LivreId = livreToUpdate.Id,
                            CategorieId = catId
                        });
                    }

                    // Retirer celles non sélectionnées
                    var toRemove = livreToUpdate.LivreCategories
                        .Where(lc => !selectedCategoryIds.Contains(lc.CategorieId))
                        .ToList();
                    if (toRemove.Count > 0)
                        donneDB.LivreCategories.RemoveRange(toRemove);

                    donneDB.SaveChanges();
                }

                ViderFormulaireLivre();
                ChargerLivres();
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }
    }
}