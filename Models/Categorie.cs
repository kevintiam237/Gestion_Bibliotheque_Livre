using System.ComponentModel.DataAnnotations;

namespace BibliothequeApp.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<LivreCategorie> LivreCategories { get; set; } = new List<LivreCategorie>();
    }
}