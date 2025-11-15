using Gestion_Bibliotheque_Livre.Models;
using Gestion_Bibliotheque_Livre.Views;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Gestion_Bibliotheque_Livre
{
    public partial class MainWindow : Window
    {
        private ResourceManager resourceManager;
        public MainWindow()
        {
            var culture = new CultureInfo("en-US"); // ou Settings
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Properties.Resources.Culture = culture;
            this.Language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);

            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(MainWindow).Assembly);
           
            InitializeComponent();
            ChargerDonnee();
            this.Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Charger la page par défaut
            NavigateToPage("Accueil");
        }
        public void UpdateUIWithResources()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            // Titre de la fenêtre
            this.Title = resourceManager.GetString("TitleApp");

            // Bouton de changement de langue
            BtnLanguage.Content = resourceManager.GetString("Language");
            BtnLanguage.Tag = currentCulture;

            // En-tête principal
            TitleTextBlock.Text = $"📚 {resourceManager.GetString("LibraryManagementSystem")}";

            //// Onglets
            BtnNavAuthors.Content = $"👤 {resourceManager.GetString("ManageAuthors")}";
            BtnNavBooks.Content = $"📖 {resourceManager.GetString("ManageBooks")}";
            BtnNavCategories.Content = $"🏷️ {resourceManager.GetString("ManageCategories")}";
            BtnNavStatistics.Content = $"📊 {resourceManager.GetString("ViewStatistics")}";
            QuickCategoriesLabel.Text = $"{resourceManager.GetString("Categories")}:";
            QuickAuthorsLabel.Text = $"{resourceManager.GetString("Authors")}:";
            QuickBooksLabel.Text = $"{resourceManager.GetString("Books")}:";
            NavHeader.Text = resourceManager.GetString("Navigation");
            QuickStatsHeader.Text = resourceManager.GetString("QuickStats");
            BtnNavHome.Content = $"🏠 {resourceManager.GetString("Home")}";

            //// Onglet Auteurs
            //GroupBoxAuthors.Header = resourceManager.GetString("AuthorManagement");
            //LabelLastName.Content = $"{resourceManager.GetString("LastName")} :";
            //LabelFirstName.Content = $"{resourceManager.GetString("FirstName")} :";
            //BtnAddAuthor.Content = $"➕ {resourceManager.GetString("Add")}";
            //BtnEditAuthor.Content = $"✏️ {resourceManager.GetString("Edit")}";
            //BtnDeleteAuthor.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            //ColAuthorLastName.Header = resourceManager.GetString("LastName");
            //ColAuthorFirstName.Header = resourceManager.GetString("FirstName");
            //ColAuthorBooksCount.Header = resourceManager.GetString("BooksCount");

            //// Onglet Livres
            //GroupBoxBooks.Header = resourceManager.GetString("BookManagement");
            //LabelBookTitle.Content = $"{resourceManager.GetString("BookTitle")} :";
            //LabelAuthor.Content = $"{resourceManager.GetString("Authors")} :";
            //LabelDate.Content = $"{resourceManager.GetString("Date")} :";
            //LabelISBN.Content = $"{resourceManager.GetString("ISBN")} :";
            //LabelPages.Content = $"{resourceManager.GetString("NumberOfPages")} :";
            //LabelCategoriesBook.Content = $"{resourceManager.GetString("Categories")} :";
            //LabelCopies.Content = $"{resourceManager.GetString("NumberOfCopies")} :";
            //BtnAddBook.Content = $"➕ {resourceManager.GetString("Add")}";
            //BtnEditBook.Content = $"✏️ {resourceManager.GetString("Edit")}";
            //BtnDeleteBook.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            //ColBookTitle.Header = resourceManager.GetString("BookTitle");
            //ColBookAuthor.Header = resourceManager.GetString("Author");
            //ColBookDate.Header = resourceManager.GetString("Date");
            //ColBookISBN.Header = resourceManager.GetString("ISBN");
            //ColBookPages.Header = resourceManager.GetString("Pages");
            //ColBookCopies.Header = resourceManager.GetString("Copies");

            //// Onglet Catégories
            //GroupBoxCategories.Header = resourceManager.GetString("CategoryManagement");
            //LabelCategoryName.Content = $"{resourceManager.GetString("CategoryName")} :";
            //BtnAddCategory.Content = $"➕ {resourceManager.GetString("Add")}";
            //BtnEditCategory.Content = $"✏️ {resourceManager.GetString("Edit")}";
            //BtnDeleteCategory.Content = $"🗑️ {resourceManager.GetString("Delete")}";
            //ColCategoryName.Header = resourceManager.GetString("CategoryName");
            //ColCategoryBooksCount.Header = resourceManager.GetString("BooksCount");

            //// Onglet Statistiques
            //StatTitle.Text = resourceManager.GetString("LibraryStatistics");
            //StatAuthorsTitle.Text = resourceManager.GetString("NumberOfAuthors");
            //StatBooksTitle.Text = resourceManager.GetString("NumberOfBooks");
            //StatCategoriesTitle.Text = resourceManager.GetString("NumberOfCategories");
            //InfoTitle.Text = $"ℹ️ {resourceManager.GetString("GeneralInformation")}";
            //InfoAuthor.Text = $"• {resourceManager.GetString("AuthorWithMostbooks")}:";
            //InfoCategory.Text = $"• {resourceManager.GetString("MostPopularCategory")}:";
            //InfoLastBook.Text = $"• {resourceManager.GetString("LastBookAdded")}:";
        }
        private void NavigateToPage(string pageName)
        {
            switch (pageName)
            {
                case "Accueil":
                    MainFrame.Navigate(new AccueilPage());
                    UpdateStatus("Page d'accueil chargée");
                    break;
                case "Authors":
                    MainFrame.Navigate(new AuthorsPage());
                    UpdateStatus("Authors page loaded");
                    break;
                case "Books":
                    MainFrame.Navigate(new BooksPage());
                    UpdateStatus("Books page loaded");
                    break;
                case "Categories":
                    MainFrame.Navigate(new CategoriesPage());
                    UpdateStatus("Categories page loaded");
                    break;
                case "Statistics":
                    MainFrame.Navigate(new StatisticsPage());
                    UpdateStatus("Statistics page loaded");
                    break;
            }

            UpdateNavigationButtons(pageName);
        }

        private void UpdateNavigationButtons(string activePage)
        {
            // Mettre à jour l'apparence des boutons de navigation
            // (ajouter des styles pour le bouton actif)
        }
        private void UpdateStatus(string message)
        {
            // TxtStatus.Text = message;
        }

        // Gestionnaires d'événements existants
        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string pageName)
            {
                NavigateToPage(pageName);
            }
        }

        private void BtnLanguage_Click(object sender, RoutedEventArgs e)
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
                BtnLanguage.Tag = newCulture;

                // Met à jour tous les textes de l'interface (titres, labels, boutons...)
                UpdateUIWithResources();
                if (MainFrame.Content is BooksPage booksPage)
                {
                    booksPage.UpdateUIWithResources();
                }
                if (MainFrame.Content is CategoriesPage categoriesPage)
                {
                    categoriesPage.UpdateUIWithResources();
                }
                if (MainFrame.Content is AuthorsPage authorsPage)
                {
                    authorsPage.UpdateUIWithResources();
                }
                if (MainFrame.Content is StatisticsPage statisticsPage)
                {
                    statisticsPage.UpdateUIWithResources();
                }
                if(MainFrame.Content is AccueilPage accueilPage)
                {
                    accueilPage.UpdateUIWithResources();
                }

                ChargerDonnee();
            }
        }

        private void ChargerDonnee()
        {
            using var donneDB = new DbContextBibliotheque();

            UpdateUIWithResources();

            if (MainFrame.Content is BooksPage books)
            {
                books.ChargerLivres();
                books.ChargerAuteurs();
                books.ChargerCategories();
            }

            int authorsCount = donneDB.Auteurs.Count();
            int booksCount = donneDB.Livres.Count();
            int categoriesCount = donneDB.Categories.Count();
            QuickAuthorsCount.Text = authorsCount.ToString();
            QuickBooksCount.Text = booksCount.ToString();
            QuickCategoriesCount.Text = categoriesCount.ToString();

        }

        /// <summary>
        /// Calcule et affiche les statistiques (comptes + infos “top”) selon la langue courante.
        /// </summary>
        private void ChargerStatistiques()
        {
            try
            {
                using var ctx = new DbContextBibliotheque();

                // Compteurs globaux
                int nbAuteurs = ctx.Auteurs.Count();
                int nbLivres = ctx.Livres.Count();
                int nbCategories = ctx.Categories.Count();

                StatAuthorsValue.Text = nbAuteurs.ToString("N0", CultureInfo.CurrentUICulture);
                StatBooksValue.Text = nbLivres.ToString("N0", CultureInfo.CurrentUICulture);
                StatCategoriesValue.Text = nbCategories.ToString("N0", CultureInfo.CurrentUICulture);

                // Auteur avec le plus de livres
                var auteurTop = ctx.Auteurs
                    .Select(a => new { a.Nom, a.Prenom, Count = a.Livres.Count })
                    .OrderByDescending(a => a.Count)
                    .ThenBy(a => a.Nom)
                    .FirstOrDefault();

                InfoAuthorValue.Text =
                    auteurTop != null && auteurTop.Count > 0
                        ? $"{auteurTop.Nom} {auteurTop.Prenom} ({auteurTop.Count})"
                        : (resourceManager.GetString("NoData") ?? "-");

                // Catégorie la plus populaire
                var categorieTop = ctx.Categories
                    .Select(c => new { c.Nom, Count = c.LivreCategories.Count })
                    .OrderByDescending(c => c.Count)
                    .ThenBy(c => c.Nom)
                    .FirstOrDefault();

                InfoCategoryvalue.Text =
                    categorieTop != null && categorieTop.Count > 0
                        ? $"{categorieTop.Nom} ({categorieTop.Count})"
                        : (resourceManager.GetString("NoData") ?? "-");

                // Dernier livre ajouté (par Id décroissant)
                var dernierLivre = ctx.Livres
                    .OrderByDescending(l => l.Id)
                    .FirstOrDefault();

                if (dernierLivre != null)
                {
                    // Utiliser la propriété formatée dépendante de la culture
                    var temp = new Livre
                    {
                        Id = dernierLivre.Id,
                        Titre = dernierLivre.Titre,
                        DatePublication = dernierLivre.DatePublication
                    };
                    string dateStr = temp.DatePublicationFormatee;
                    InfoLastBookValue.Text = $"{dernierLivre.Titre} — {dateStr}";
                }
                else
                {
                    InfoLastBookValue.Text = resourceManager.GetString("NoData") ?? "-";
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur inattendue sur les stats, on affiche dans la barre d'erreur
                AfficherErreur("ErrorUnexpected", ex.Message);
            }
        }
    }
}