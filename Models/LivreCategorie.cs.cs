namespace BibliothequeApp.Models
{
    public class LivreCategorie
    {
        public int LivreId { get; set; }
        public int CategorieId { get; set; }

        // Navigation properties
        public virtual Livre Livre { get; set; } = null!;
        public virtual Categorie Categorie { get; set; } = null!;
    }
}