using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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
        public virtual DetailsLivre DetailsLivre { get; set; } = null!;
        public virtual ICollection<LivreCategorie> LivreCategories { get; set; } = new List<LivreCategorie>();

        // Propriété calculée pour le format de date personnalisé
        [NotMapped]
        public string DatePublicationFormatee
        {
            get
            {
                var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                int day = DatePublication.Day;
                string month = DatePublication.ToString("MMMM", culture);
                int year = DatePublication.Year;

                if (culture.Name.StartsWith("fr"))
                {
                    string dayFormat = day == 1 ? "1er" : day.ToString();
                    return $"{dayFormat} {month} {year}";
                }
                else
                {
                    string suffix = GetOrdinalSuffix(day);
                    return $"{month} {day}{suffix}, {year}";
                }
            }
        }

        // Méthode privée pour obtenir le suffixe ordinal en anglais
        private static string GetOrdinalSuffix(int day)
        {
            if (day >= 11 && day <= 13)
                return "th";

            return (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }
    }
}
