using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gestion_Bibliotheque_Livre.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Bibliotheque_Livre.Views
{
    /// <summary>
    /// Logique d'interaction pour AccueilPage.xaml
    /// </summary>
    public partial class AccueilPage : UserControl
    {
        private ResourceManager resourceManager;

        public AccueilPage()
        {
            InitializeComponent();

            // Initialiser le gestionnaire de ressources
            resourceManager = new ResourceManager(
                "Gestion_Bibliotheque_Livre.Properties.Resources",
                typeof(MainWindow).Assembly);

            // Mettre à jour l'interface avec les ressources
            UpdateUIWithResources();

        }
      
         public void UpdateUIWithResources() 
         { 
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            WelcomeTitleTextBlock.Text = $"📚 {resourceManager.GetString("WelcomeTitle")}";
            WelcomeSubtitleTextBlock.Text = resourceManager.GetString("WelcomeSubtitle");
            ExploreTitleTextBlock.Text = $"🌟 {resourceManager.GetString("ExploreTitle")}";
            ExploreDescriptionTextBlock.Text = resourceManager.GetString("ExploreDescription");
            Feature1TitleTextBlock.Text = resourceManager.GetString("Feature1Title");
            Feature1DescriptionTextBlock.Text = resourceManager.GetString("Feature1Description");
            Feature2TitleTextBlock.Text = resourceManager.GetString("Feature2Title");
            Feature2DescriptionTextBlock.Text = resourceManager.GetString("Feature2Description");
            Feature3TitleTextBlock.Text = resourceManager.GetString("Feature3Title");
            Feature3DescriptionTextBlock.Text = resourceManager.GetString("Feature3Description");
            QuoteTextTextBlock.Text = resourceManager.GetString("QuoteText");
            QuoteAuthorTextBlock.Text = resourceManager.GetString("QuoteAuthor");
            NavigationTitleTextBlock.Text = $"🎯 {resourceManager.GetString("NavigationTitle")}";
            NavigationDescriptionTextBlock.Text = resourceManager.GetString("NavigationDescription");
            StatusMessageTextBlock.Text = resourceManager.GetString("StatusMessage");

         }
    }
}
    

