using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliothequeApp.Models
{
    public class Auteur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; } = null!;

        // Propriété calculée pour afficher le nom complet
        [NotMapped]
        public string NomComplet => $"{Prenom} {Nom}";

        // Navigation properties
        public virtual ICollection<Livre> Livres { get; set; } = new List<Livre>();
    }
}