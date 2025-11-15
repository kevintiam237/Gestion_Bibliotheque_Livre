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
            resourceManager = new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(MainWindow).Assembly);
            InitializeComponent();
            UpdateUIWithResources();

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
}