# Gestion_Bibliothèque_Livre

Application WPF (.NET 10, C# 14) pour la gestion d’une bibliothèque: auteurs, livres, catégories et statistiques.

## Objectifs et choix techniques

- WPF + XAML: interface riche, styles globaux, thèmes modernes et composants desktop natifs Windows.
- C# + .NET 10: performance, sécurité et API modernes pour une base pérenne.
- Entity Framework Core: accès aux données productif avec LINQ, tracking des entités et navigation.
- Localisation (i18n): bascule dynamique des textes via `ResourceManager` et ressources `Properties.Resources`.
- Simplicité d’architecture: pages WPF avec code-behind clair et direct, facilement migrable vers MVVM.

## Structure (principale)

- `App.xaml`: ressources globales (couleurs, brushes, styles, templates).
- `Views/AccueilPage.*`: page d’accueil (messages, navigation).
- `Views/BooksPage.*`: gestion des livres (CRUD, sélection d’auteur/catégories).
- `Views/AuthorsPage.*`: gestion des auteurs (CRUD).
- `Views/CategoriesPage.*`: gestion des catégories (CRUD).
- `Views/StatisticsPage.*`: statistiques agrégées.

Des modèles et le `DbContextBibliotheque` (EF Core) définissent les entités: `Auteur`, `Livre`, `Categorie`, `LivreCategorie` (N-N), `DetailsLivre`.

## UI/UX: styles et ergonomie

- Styles globaux (App.xaml):
  - Palette moderne (primaires, accent, info, warning, danger).
  - Boutons (primaire, succès, danger, navigation).
  - Contrôles formulaire (TextBox/ComboBox/DatePicker) modernes.
  - DataGrid modernisé (en-têtes, lignes, cellules, survol/selection).
- DataGrid: sélection pleine ligne, clic sur toute la rangée (pas seulement le texte).
  - `SelectionUnit="FullRow"` et `SelectionMode="Single"` pour une sélection explicite.
  - `IsReadOnly="True"` pour éviter l’édition inline accidentelle.
  - `HorizontalContentAlignment="Stretch"` dans `DataGridCell` pour rendre toute la cellule cliquable.
- Messages d’état/erreur: `Border` dédié, icône et style d’alerte cohérent.

Pourquoi ces choix:
- Cohérence visuelle et accessibilité.
- Surface cliquable généreuse et comportements prévisibles.
- Séparation claire des styles et de la logique.

## Internationalisation (i18n)

- Chaque page possède `UpdateUIWithResources()` qui lit les chaînes via:
  - `new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(CettePage).Assembly)`
- Exemple (Accueil):
# Gestion_Bibliothèque_Livre

Application WPF (.NET 10, C# 14) pour la gestion d’une bibliothèque: auteurs, livres, catégories et statistiques.

## Objectifs et choix techniques

- WPF + XAML: interface riche, styles globaux, thèmes modernes et composants desktop natifs Windows.
- C# + .NET 10: performance, sécurité et API modernes pour une base pérenne.
- Entity Framework Core: accès aux données productif avec LINQ, tracking des entités et navigation.
- Localisation (i18n): bascule dynamique des textes via `ResourceManager` et ressources `Properties.Resources`.
- Simplicité d’architecture: pages WPF avec code-behind clair et direct, facilement migrable vers MVVM.

## Structure (principale)

- `App.xaml`: ressources globales (couleurs, brushes, styles, templates).
- `Views/AccueilPage.*`: page d’accueil (messages, navigation).
- `Views/BooksPage.*`: gestion des livres (CRUD, sélection d’auteur/catégories).
- `Views/AuthorsPage.*`: gestion des auteurs (CRUD).
- `Views/CategoriesPage.*`: gestion des catégories (CRUD).
- `Views/StatisticsPage.*`: statistiques agrégées.

Des modèles et le `DbContextBibliotheque` (EF Core) définissent les entités: `Auteur`, `Livre`, `Categorie`, `LivreCategorie` (N-N), `DetailsLivre`.

## UI/UX: styles et ergonomie

- Styles globaux (App.xaml):
  - Palette moderne (primaires, accent, info, warning, danger).
  - Boutons (primaire, succès, danger, navigation).
  - Contrôles formulaire (TextBox/ComboBox/DatePicker) modernes.
  - DataGrid modernisé (en-têtes, lignes, cellules, survol/selection).
- DataGrid: sélection pleine ligne, clic sur toute la rangée (pas seulement le texte).
  - `SelectionUnit="FullRow"` et `SelectionMode="Single"` pour une sélection explicite.
  - `IsReadOnly="True"` pour éviter l’édition inline accidentelle.
  - `HorizontalContentAlignment="Stretch"` dans `DataGridCell` pour rendre toute la cellule cliquable.
- Messages d’état/erreur: `Border` dédié, icône et style d’alerte cohérent.

Pourquoi ces choix:
- Cohérence visuelle et accessibilité.
- Surface cliquable généreuse et comportements prévisibles.
- Séparation claire des styles et de la logique.

## Internationalisation (i18n)

- Chaque page possède `UpdateUIWithResources()` qui lit les chaînes via:
  - `new ResourceManager("Gestion_Bibliotheque_Livre.Properties.Resources", typeof(CettePage).Assembly)`
- Exemple (Accueil):
  - `WelcomeTitleTextBlock.Text = $"📚 {resourceManager.GetString("WelcomeTitle")}";`
- Pour changer la langue à chaud:
  - ajuster `Thread.CurrentThread.CurrentUICulture` puis appeler `UpdateUIWithResources()`/`ApplyLanguage()` sur les pages affichées.

## Accès aux données (EF Core)

- Requêtes LINQ avec `Include` pour charger les relations (ex: `Livre.Auteur`, `Livre.LivreCategories.Categorie`).
- Patterns CRUD simples et lisibles (validation + messages d’erreur).
- Joins N-N gérés via l’entité de jointure `LivreCategorie`.

Pourquoi EF Core:
- Réduction du code boilerplate.
- Projections LINQ pour afficher les DTO anonymes dans les DataGrid (ex: `NombreLivres` par auteur/catégorie).
- Évolutif vers migrations et bases multiples.

## Points notables

- Problème “il faut cliquer sur le texte pour sélectionner la ligne”: résolu globalement par:
  - `SelectionUnit="FullRow"` + `SelectionMode="Single"` + `IsReadOnly="True"` (sur les DataGrid).
  - `HorizontalContentAlignment="Stretch"` et `Padding` adaptés sur `DataGridCell`.
- Lisibilité du contenu: padding à gauche augmenté pour éviter un rendu “serré”.

## Prérequis

- Windows, Visual Studio 2026 (ou supérieur).
- SDK .NET 10 installé.
- Source de données configurée dans `DbContextBibliotheque` (adapter la chaîne de connexion selon votre environnement).

## Construction et exécution

1. Restaurer les packages NuGet.
2. Compiler la solution en Configuration Debug.
3. Lancer l’application (projet WPF de démarrage).
4. Vérifier l’accès à la base de données via `DbContextBibliotheque`.

## Améliorations futures (roadmap)

- Passage progressif vers MVVM:
  - `ObservableCollection<>`, `INotifyPropertyChanged`, `ICommand`.
  - Tests unitaires de la logique métier (ViewModels) sans UI.
- Migrations EF Core et configuration externalisée (appsettings.json).
- Thème sombre et bascule de thème dynamique.
- Virtualisation des DataGrid pour gros volumes.

## Contributions

- Respecter les styles et conventions en place (XAML et C#).
- Factoriser les chaînes dans `Properties.Resources`.
- Préserver les styles/ressources globaux (`App.xaml`) pour la cohérence visuelle.

	# Gestion_Bibliotheque_Livre
