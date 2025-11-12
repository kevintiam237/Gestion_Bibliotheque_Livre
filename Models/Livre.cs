using System.ComponentModel.DataAnnotations;

namespace BibliothequeApp.Models
{
    public class Livre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = null!;

        public DateTime DatePublication { get; set; } = DateTime.Now;

        // Foreign keys
        public int AuteurId { get; set; }

        // Navigation properties
        public virtual Auteur Auteur { get; set; } = null!;
        public virtual DetailsLivre Details { get; set; } = null!;
        public virtual ICollection<LivreCategorie> LivreCategories { get; set; } = new List<LivreCategorie>();
    }
}