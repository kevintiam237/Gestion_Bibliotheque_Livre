using BibliothequeApp.Models;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Gestion_Bibliotheque_Livre
{
    /// <summary>
    /// Fenêtre principale de l'application de gestion de bibliothèque.
    /// Gère l'affichage, la traduction, la saisie des données et la communication avec la base de données.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Gestionnaire des ressources localisées (fichiers .resx)
        private ResourceManager resourceManager;

        // Stocke la clé de ressource du message d'erreur actuel (ex: "ErrorBookTitle")
        private string? currentErrorKey;

        // Stocke un paramètre additionnel éventuel pour le message d'erreur (ex: le message d'exception)
        private string? currentErrorParams;

        /// <summary>
        /// Constructeur de la fenêtre principale.
        /// Initialise la culture par défaut, le ResourceManager, et charge les données.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialise le gestionnaire de ressources pour charger les chaînes localisées
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(MainWindow).Assembly);

            // Met à jour l'interface avec les textes de la langue par défaut
            UpdateUIWithResources();

            // Lorsque la fenêtre est chargée, on déclenche le chargement des données
            Loaded += ChargerDonnees;
        }

        /// <summary>
        /// Événement déclenché au chargement de la fenêtre.
        /// Charge les livres, auteurs et catégories au démarrage.
        /// </summary>
        private void ChargerDonnees(object sender, RoutedEventArgs e)
        {
            ChargerLivre();
            ChargerAuteurs();
            ChargerCategories();
        }

        /// <summary>
        /// Gestionnaire du bouton de changement de langue.
        /// Bascule entre fr-FR et en-US, met à jour la culture, et recharge tout l'interface.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string currentCulture)
            {
                // Inverse la langue : fr-FR ↔ en-US
                string newCulture = currentCulture == "fr-FR" ? "en-US" : "fr-FR";

                // Applique la nouvelle culture à la thread courante
                var cultureInfo = new CultureInfo(newCulture);
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Properties.Resources.Culture = cultureInfo;

                // Met à jour l'icône et le texte du bouton
                LangToggleButton.Tag = newCulture;

                // Met à jour tous les textes de l'interface (titres, labels, boutons...)
                UpdateUIWithResources();

                // Si un message d'erreur est actuellement affiché, on le rafraîchit avec la nouvelle langue
                if (ErrorMessageBorder.Visibility == Visibility.Visible && !string.IsNullOrEmpty(currentErrorKey))
                {
                    RafraichirMessageErreur();
                }

                // Recharge les données depuis la base avec la nouvelle langue
                ChargerLivre();
                ChargerAuteurs();
                ChargerCategories();
            }
        }

        /// <summary>
        /// Met à jour tous les textes de l'interface utilisateur selon la culture actuelle.
        /// Utilise les ressources (.resx) pour afficher les chaînes traduites.
        /// </summary>
        public void UpdateUIWithResources()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            // Titre de la fenêtre
            this.Title = resourceManager.GetString("TitleApp");

            // Bouton de changement de langue
            LangToggleButton.Content = resourceManager.GetString("Language");
            LangToggleButton.Tag = currentCulture;

            // En-tête principal
            TitleTextBlock.Text = $"📚 {resourceManager.GetString("LibraryManagementSystem")}";

            // Onglets
            TabAuthors.Header = $"👤 {resourceManager.GetString("Authors")}";
            TabBooks.Header = $"📖 {resourceManager.GetString("Books")}";
            TabCategories.Header = $"🏷️ {resourceManager.GetString("Categories")}";
            TabStatistics.Header = $"📊 {resourceManager.GetString("Statistics")}";

            // Onglet Auteurs
            GroupBoxAuthors.Header = resourceManager.GetString("AuthorManagement");
            LabelLastName.Content = $"{resourceManager.GetString("LastName")} :";
            LabelFirstName.Content = $"{resourceManager.GetString("FirstName")} :";
            BtnAddAuthor.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditAuthor.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteAuthor.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            ColAuthorLastName.Header = resourceManager.GetString("LastName");
            ColAuthorFirstName.Header = resourceManager.GetString("FirstName");
            ColAuthorBooksCount.Header = resourceManager.GetString("BooksCount");

            // Onglet Livres
            GroupBoxBooks.Header = resourceManager.GetString("BookManagement");
            LabelBookTitle.Content = $"{resourceManager.GetString("BookTitle")} :";
            LabelAuthor.Content = $"{resourceManager.GetString("Authors")} :";
            LabelDate.Content = $"{resourceManager.GetString("Date")} :";
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

            // Onglet Catégories
            GroupBoxCategories.Header = resourceManager.GetString("CategoryManagement");
            LabelCategoryName.Content = $"{resourceManager.GetString("CategoryName")} :";
            BtnAddCategory.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditCategory.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteCategory.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            ColCategoryName.Header = resourceManager.GetString("CategoryName");
            ColCategoryBooksCount.Header = resourceManager.GetString("BooksCount");

            // Onglet Statistiques
            StatTitle.Text = resourceManager.GetString("LibraryStatistics");
            StatAuthorsTitle.Text = resourceManager.GetString("NumberOfAuthors");
            StatBooksTitle.Text = resourceManager.GetString("NumberOfBooks");
            StatCategoriesTitle.Text = resourceManager.GetString("NumberOfCategories");
            InfoTitle.Text = $"ℹ️ {resourceManager.GetString("GeneralInformation")}";
            InfoAuthor.Text = $"• {resourceManager.GetString("AuthorWithMostbooks")}:";
            InfoCategory.Text = $"• {resourceManager.GetString("MostPopularCategory")}:";
            InfoLastBook.Text = $"• {resourceManager.GetString("LastBookAdded")}:";
        }

        /// <summary>
        /// Charge la liste des livres depuis la base de données.
        /// Inclut les relations : Auteur, DetailsLivre et LivreCategories → Categorie.
        /// Recrée les objets Livre pour forcer le recalcul de DatePublicationFormatee selon la langue.
        /// </summary>
        private void ChargerLivre()
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
        private void ChargerAuteurs()
        {
            using (var ListeDesLivres = new DbContextBibliotheque())
            {
                var auteurs = ListeDesLivres.Auteurs
                    .OrderBy(a => a.Nom)
                    .ThenBy(a => a.Prenom)
                    .ToList();

                // Vide le ComboBox
                ComboBoxAuthorSelect.Items.Clear();

                // Ajoute l'option par défaut
                ComboBoxAuthorSelect.Items.Add(new ComboBoxItem
                {
                    Content = resourceManager.GetString("SelectAuthor"),
                    Tag = null
                });

                // Ajoute chaque auteur avec son nom complet et son ID en Tag
                foreach (var auteur in auteurs)
                {
                    ComboBoxAuthorSelect.Items.Add(new ComboBoxItem
                    {
                        Content = auteur.NomComplet,
                        Tag = auteur.Id
                    });
                }

                // Sélectionne la première option (par défaut)
                ComboBoxAuthorSelect.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Charge la liste des catégories depuis la base et les affiche dans le ComboBoxCategoriesSelect.
        /// Ajoute une option par défaut "Sélectionner une catégorie".
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
                        NombreLivres = c.LivreCategories.Count()
                    })
                    .ToList();

                // Vide le ComboBox
                ComboBoxCategoriesSelect.Items.Clear();
                DataGridCategories.ItemsSource = categories;

                // Ajoute l'option par défaut
                ComboBoxCategoriesSelect.Items.Add(new ComboBoxItem
                {
                    Content = resourceManager.GetString("SelectCategory"),
                    Tag = null
                });

                // Ajoute chaque catégorie avec son nom et son ID en Tag
                foreach (var categorie in categories)
                {
                    ComboBoxCategoriesSelect.Items.Add(new ComboBoxItem
                    {
                        Content = categorie.Nom,
                        Tag = categorie.Id
                    });
                }

                // Sélectionne la première option (par défaut)
                ComboBoxCategoriesSelect.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gère la sélection d’un livre dans le DataGrid.
        /// Remplit les champs du formulaire avec les données du livre sélectionné.
        /// Sélectionne automatiquement l’auteur et la catégorie dans les ComboBox.
        /// </summary>
        private void dgvBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedLivre = dgvBooks.SelectedItem as Livre;

            if (selectedLivre != null)
            {
                // Remplit les champs du formulaire
                TxtBookTitle.Text = selectedLivre.Titre;
                TxtISBN.Text = selectedLivre.DetailsLivre?.ISBN ?? string.Empty;
                TxtPages.Text = selectedLivre.DetailsLivre?.NombrePages.ToString() ?? "1";
                TxtCopies.Text = selectedLivre.DetailsLivre?.NombreExemplaires.ToString() ?? "1";
                DatePickerPublication.SelectedDate = selectedLivre.DatePublication;

                // Sélectionne l’auteur correspondant dans le ComboBox
                foreach (ComboBoxItem item in ComboBoxAuthorSelect.Items)
                {
                    if (item.Tag is int auteurId && auteurId == selectedLivre.AuteurId)
                    {
                        ComboBoxAuthorSelect.SelectedItem = item;
                        break;
                    }
                }

                // Sélectionne la catégorie correspondante dans le ComboBox
                foreach (ComboBoxItem item in ComboBoxCategoriesSelect.Items)
                {
                    if (item.Tag is int categorieId && selectedLivre.LivreCategories.Any(lc => lc.CategorieId == categorieId))
                    {
                        ComboBoxCategoriesSelect.SelectedItem = item;
                        break;
                    }
                }
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
                MasquerErreur(); // Cache tout message d'erreur précédent

                // Valide les champs du formulaire
                if (!ValiderFormulaireLivre())
                    return;

                // Récupère l'ID de l'auteur sélectionné
                if (!(ComboBoxAuthorSelect.SelectedItem is ComboBoxItem authorItem) || !(authorItem.Tag is int auteurId))
                {
                    AfficherErreur("ErrorAuthorRequired");
                    return;
                }

                // Récupère l'ID de la catégorie sélectionnée
                if (!(ComboBoxCategoriesSelect.SelectedItem is ComboBoxItem catItem) || !(catItem.Tag is int categorieId))
                {
                    AfficherErreur("ErrorCategoryRequired");
                    return;
                }

                // Vérifie si un livre avec le même titre et auteur existe déjà
                using (var ListeDesLivres = new DbContextBibliotheque())
                {
                    var livreExiste = ListeDesLivres.Livres
                        .FirstOrDefault(l => l.Titre == TxtBookTitle.Text.Trim() && l.AuteurId == auteurId);

                    if (livreExiste != null)
                    {
                        AfficherErreur("ErrorBookExists");
                        return;
                    }

                    // Crée un nouveau livre
                    var nouveauLivre = new Livre
                    {
                        Titre = TxtBookTitle.Text.Trim(),
                        DatePublication = DatePickerPublication.SelectedDate ?? DateTime.Now,
                        AuteurId = auteurId,
                        DetailsLivre = new DetailsLivre
                        {
                            ISBN = TxtISBN.Text,
                            NombrePages = int.TryParse(TxtPages.Text, out int pages) ? pages : 0,
                            NombreExemplaires = int.TryParse(TxtCopies.Text, out int copies) ? copies : 0
                        }
                    };

                    ListeDesLivres.Livres.Add(nouveauLivre);
                    ListeDesLivres.SaveChanges();

                    // Lie le livre à la catégorie sélectionnée
                    ListeDesLivres.LivreCategories.Add(new LivreCategorie
                    {
                        LivreId = nouveauLivre.Id,
                        CategorieId = categorieId
                    });
                    ListeDesLivres.SaveChanges();

                    // Recharge la liste des livres pour afficher le nouveau
                    ChargerLivre();
                }

                // Vide le formulaire après ajout
                ViderFormulaireLivre();

                // Affiche un message de succès
                MessageBox.Show(
                    resourceManager.GetString("SuccessBookAdded") ?? "Le livre a été ajouté avec succès !",
                    resourceManager.GetString("Success") ?? "Succès",
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
        /// Valide tous les champs du formulaire d'ajout de livre.
        /// Affiche un message d'erreur si un champ est invalide.
        /// Retourne true si tout est valide, false sinon.
        /// </summary>
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

            if (ComboBoxCategoriesSelect.SelectedIndex <= 0)
            {
                AfficherErreur("ErrorCategoryRequired");
                return false;
            }

            return true; // Tous les champs sont valides
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
            ComboBoxCategoriesSelect.SelectedIndex = 0;
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
                        ChargerLivre();
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
                ChargerLivre();
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

                // Validation UI
                if (!ValiderFormulaireLivre())
                    return;

                if (!(ComboBoxAuthorSelect.SelectedItem is ComboBoxItem authorItem) || !(authorItem.Tag is int auteurId))
                {
                    AfficherErreur("ErrorAuthorRequired");
                    return;
                }

                if (!(ComboBoxCategoriesSelect.SelectedItem is ComboBoxItem catItem) || !(catItem.Tag is int categorieId))
                {
                    AfficherErreur("ErrorCategoryRequired");
                    return;
                }

                var selectedLivre = dgvBooks.SelectedItem as Livre;
                if (selectedLivre == null)
                {
                    AfficherErreur("ErrorBookTitle"); // Ou une clé dédiée: ErrorSelectBook
                    return;
                }

                using (var ListeDesLivres = new DbContextBibliotheque())
                {
                    // Recharger l’entité dans le contexte (instance suivie unique)
                    var livreToUpdate = ListeDesLivres.Livres
                        .Include(l => l.DetailsLivre)
                        .Include(l => l.LivreCategories)
                        .FirstOrDefault(l => l.Id == selectedLivre.Id);

                    if (livreToUpdate == null)
                    {
                        ChargerLivre();
                        return;
                    }

                    // Éviter les doublons Titre + Auteur
                    string nouveauTitre = TxtBookTitle.Text.Trim();
                    bool existeDeja = ListeDesLivres.Livres
                        .Any(l => l.Id != livreToUpdate.Id && l.Titre == nouveauTitre && l.AuteurId == auteurId);
                    if (existeDeja)
                    {
                        AfficherErreur("ErrorBookExists");
                        return;
                    }

                    // Mettre à jour les propriétés de base
                    livreToUpdate.Titre = nouveauTitre;
                    livreToUpdate.DatePublication = DatePickerPublication.SelectedDate ?? DateTime.Now;
                    livreToUpdate.AuteurId = auteurId;

                    // Mettre à jour les détails
                    if (livreToUpdate.DetailsLivre == null)
                    {
                        livreToUpdate.DetailsLivre = new DetailsLivre
                        {
                            LivreId = livreToUpdate.Id
                        };
                    }
                    livreToUpdate.DetailsLivre.ISBN = TxtISBN.Text;
                    livreToUpdate.DetailsLivre.NombrePages = int.TryParse(TxtPages.Text, out int pages) ? pages : 0;
                    livreToUpdate.DetailsLivre.NombreExemplaires = int.TryParse(TxtCopies.Text, out int copies) ? copies : 0;

                    // Mettre à jour la (seule) catégorie liée
                    if (livreToUpdate.LivreCategories == null)
                        livreToUpdate.LivreCategories = new List<LivreCategorie>();

                    // Retirer les catégories différentes de celle sélectionnée
                    var toRemove = livreToUpdate.LivreCategories
                        .Where(lc => lc.CategorieId != categorieId)
                        .ToList();
                    if (toRemove.Count > 0)
                        ListeDesLivres.LivreCategories.RemoveRange(toRemove);

                    // Ajouter la catégorie sélectionnée si absente
                    if (!livreToUpdate.LivreCategories.Any(lc => lc.CategorieId == categorieId))
                    {
                        livreToUpdate.LivreCategories.Add(new LivreCategorie
                        {
                            LivreId = livreToUpdate.Id,
                            CategorieId = categorieId
                        });
                    }

                    ListeDesLivres.SaveChanges();
                    ViderFormulaireLivre();
                }

                // Rafraîchir l’affichage
                ChargerLivre();
            }
            catch (Exception ex)
            {
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }

        // Méthode pour ajouter une catégorie
        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            var nom = TxtCategoryName.Text?.Trim();

            if (string.IsNullOrWhiteSpace(nom))
            {
                MessageBox.Show("Veuillez saisir un nom de catégorie.", "Validation", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using var ListeDesLivres = new DbContextBibliotheque();

                // Refus des doublons (sensibles à la casse, ajustez si besoin)
                if (ListeDesLivres.Categories.Any(c => c.Nom == nom))
                {
                    MessageBox.Show("Cette catégorie existe déjà.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ListeDesLivres.Categories.Add(new Categorie { Nom = nom });
                ListeDesLivres.SaveChanges();

                TxtCategoryName.Clear();
                ChargerCategories(); // rafraîchir la grille
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour éditer une catégorie
        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
            if (element == null) { MessageBox.Show("Sélectionnez une catégorie."); return; }

            var type = element.GetType();
            var idObj = type.GetProperty("Id")?.GetValue(element);
            if (idObj is not int id) { MessageBox.Show("Sélection invalide."); return; }

            var nouveauNom = TxtCategoryName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(nouveauNom)) { MessageBox.Show("Saisissez un nom."); return; }

            using var ListeDesLivres = new DbContextBibliotheque();

            if (ListeDesLivres.Categories.Any(c => c.Nom == nouveauNom && c.Id != id))
            {
                MessageBox.Show("Une autre catégorie porte déjà ce nom.");
                return;
            }

            var categorie = ListeDesLivres.Categories.Find(id);
            if (categorie == null) { MessageBox.Show("Catégorie introuvable."); return; }

            categorie.Nom = nouveauNom;
            ListeDesLivres.SaveChanges();
            ChargerCategories();
        }

        // Méthode pour supprimer une catégorie
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
            if (element == null) { MessageBox.Show("Sélectionnez une catégorie."); return; }

            var type = element.GetType();
            var idObj = type.GetProperty("Id")?.GetValue(element);
            var nom = type.GetProperty("Nom")?.GetValue(element)?.ToString();
            if (idObj is not int id) { MessageBox.Show("Sélection invalide."); return; }

            if (MessageBox.Show($"Supprimer '{nom}' ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using var ListeDesLivres = new DbContextBibliotheque();
            var categorie = ListeDesLivres.Categories.Find(id);
            if (categorie == null) { MessageBox.Show("Catégorie introuvable."); return; }

            ListeDesLivres.Categories.Remove(categorie);
            ListeDesLivres.SaveChanges();

            TxtCategoryName.Clear();
            ChargerCategories();
        }

        // Gestionnaire de sélection dans le DataGrid des catégories
        private void DataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
            if (element == null) return;

            // Les éléments sont des types anonymes { Id, Nom, NombreLivres } → on lit via réflexion
            var nom = element.GetType().GetProperty("Nom")?.GetValue(element)?.ToString();
            TxtCategoryName.Text = nom ?? string.Empty;
        }
    }
}