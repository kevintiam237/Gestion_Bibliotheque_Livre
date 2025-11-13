using System.Globalization;
using System.Net.NetworkInformation;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;
using BibliothequeApp.Models;

namespace Gestion_Bibliotheque_Livre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ResourceManager resourceManager;

        public MainWindow()
        {
            InitializeComponent();
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(MainWindow).Assembly);
            UpdateUIWithResources();
            ChargerCategories();
            BtnAddCategory.Click += BtnAddCategory_Click;
        }
        // Méthode pour charger les catégories avec le nombre de livre
        private void ChargerCategories()
        {
            using var contexte = new DbContextBibliotheque();

            var categories = contexte.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Nom,
                    NombreLivres = c.LivreCategories.Count
                })
                .OrderBy(c => c.Nom)
                .ToList();

            DataGridCategories.ItemsSource = categories;
        }
        // Méthode pour éditer les champs
        private async void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
                if (element == null) { MessageBox.Show(resourceManager.GetString("Msg_SelectCategory"), resourceManager.GetString("Title_Info"), MessageBoxButton.OK, MessageBoxImage.Information); return; }

            var type = element.GetType();
            var idObj = type.GetProperty("Id")?.GetValue(element);
            if (idObj is not int id) { MessageBox.Show(resourceManager.GetString("Msg_InvalidSelection"), resourceManager.GetString("Title_Info"), MessageBoxButton.OK, MessageBoxImage.Information); return; }

            var nouveauNom = TxtCategoryName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(nouveauNom)) { MessageBox.Show(resourceManager.GetString("Msg_EnterName"), resourceManager.GetString("Title_Validation"), MessageBoxButton.OK, MessageBoxImage.Information); return; }

            using var contexte = new DbContextBibliotheque();

            if (await contexte.Categories.AnyAsync(c => c.Nom == nouveauNom && c.Id != id))
            {
                MessageBox.Show(resourceManager.GetString("Msg_OtherNameExists"), resourceManager.GetString("Title_Info"), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var categorie = await contexte.Categories.FindAsync(id);
            if (categorie == null) { MessageBox.Show(resourceManager.GetString("Msg_CategoryNotFound"), resourceManager.GetString("Title_Error"), MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            categorie.Nom = nouveauNom;
            await contexte.SaveChangesAsync();
            ChargerCategories();
        }
        // Méthode pour supprimer la catégorie sélectionnée (avec confirmation)
        private void DataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
            if (element == null) return;

            // Les éléments sont des types anonymes { Id, Nom, NombreLivres } → on lit via réflexion
            var nom = element.GetType().GetProperty("Nom")?.GetValue(element)?.ToString();
            TxtCategoryName.Text = nom ?? string.Empty;
        }
        // Méthode pour ajouter une nouvelle catégorie
        private async void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var element = DataGridCategories.SelectedItem;
            if (element == null) { MessageBox.Show("Sélectionnez une catégorie."); return; }

            var type = element.GetType();
            var idObj = type.GetProperty("Id")?.GetValue(element);
            var nom = type.GetProperty("Nom")?.GetValue(element)?.ToString();
            if (idObj is not int id) { MessageBox.Show("Sélection invalide."); return; }

            if (MessageBox.Show(
                    string.Format(resourceManager.GetString("Msg_DeleteConfirm")!, nom),
                    resourceManager.GetString("Title_Confirm"),
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using var contexte = new DbContextBibliotheque();
            var categorie = await contexte.Categories.FindAsync(id);
            if (categorie == null) { MessageBox.Show("Catégorie introuvable."); return; }

            contexte.Categories.Remove(categorie);
            await contexte.SaveChangesAsync();

            TxtCategoryName.Clear();
            ChargerCategories();
        }
        // Méthode pour ajouter categorie
        private async void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            var nom = TxtCategoryName.Text?.Trim();

            if (string.IsNullOrWhiteSpace(nom))
            {
                MessageBox.Show(resourceManager.GetString("Msg_EnterCategoryName"), resourceManager.GetString("Title_Validation"), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using var contexte = new DbContextBibliotheque();

                // Refus des doublons (sensibles à la caisse, ajustez si besoin)
                if (await contexte.Categories.AnyAsync(c => c.Nom == nom))
                {
                    MessageBox.Show(resourceManager.GetString("Msg_NameExists"), resourceManager.GetString("Title_Info"), MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                contexte.Categories.Add(new Categorie { Nom = nom });
                await contexte.SaveChangesAsync();

                TxtCategoryName.Clear();
                ChargerCategories(); // rafraîchir la grille
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(resourceManager.GetString("Msg_AddError")!, ex.Message), resourceManager.GetString("Title_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string currentCulture)
            {
                string newCulture = currentCulture == "fr-FR" ? "en-US" : "fr-FR";

                var cultureInfo = new CultureInfo(newCulture);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Properties.Resources.Culture = cultureInfo;

                // Mettre à jour le tag du bouton
                LangToggleButton.Tag = newCulture;

                // Mettre à jour toute l'interface utilisateur
                UpdateUIWithResources();
            }
        }

        // Méthode pour initialiser/mettre à jour la langue
        public void UpdateUIWithResources()
        {
            string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

            // Titre de la fenêtre
            this.Title = resourceManager.GetString("TitleApp");

            // Bouton de langue
            LangToggleButton.Content = resourceManager.GetString("Language");
            LangToggleButton.Tag = currentCulture;

            // En-tête
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

            // DataGrid Auteurs - En-têtes
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
            BtnManageCategories.Content = resourceManager.GetString("ManageCategories");
            BtnAddBook.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditBook.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteBook.Content = $"🗑️ {resourceManager.GetString("Delete")}";

            // ComboBox Auteur
            ComboBoxAuthorSelect.Items.Clear();
            ComboBoxAuthorSelect.Items.Add(resourceManager.GetString("SelectAuthor"));
            ComboBoxAuthorSelect.SelectedIndex = 0;

            // DataGrid Livres - En-têtes
            ColBookTitle.Header = resourceManager.GetString("BookTitle");
            ColBookAuthor.Header = resourceManager.GetString("Author");
            ColBookDate.Header = resourceManager.GetString("Date");
            ColBookISBN.Header = resourceManager.GetString("ISBN");
            ColBookPages.Header = resourceManager.GetString("Pages");

            // Onglet Catégories
            GroupBoxCategories.Header = resourceManager.GetString("CategoryManagement");
            LabelCategoryName.Content = $"{resourceManager.GetString("CategoryName")} :";
            BtnAddCategory.Content = $"➕ {resourceManager.GetString("Add")}";
            BtnEditCategory.Content = $"✏️ {resourceManager.GetString("Edit")}";
            BtnDeleteCategory.Content = $"🗑️ {resourceManager.GetString("Delete")}";

            // DataGrid Catégories - En-têtes
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
    }
}