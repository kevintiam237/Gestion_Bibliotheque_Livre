using System.ComponentModel.DataAnnotations;

namespace BibliothequeApp.Models
{
    public class DetailsLivre
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int NombrePages { get; set; }

        [Range(0, int.MaxValue)]
        public int NombreExemplaires { get; set; } = 1;

        // Foreign key
        public int LivreId { get; set; }

        // Navigation property
        public virtual Livre Livre { get; set; } = null!;
    }
}