using System.Globalization;
using System.Net.NetworkInformation;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

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

            // Initialise le ResourceManager
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(MainWindow).Assembly);

            // Charge la langue par défaut
            UpdateUIWithResources();
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