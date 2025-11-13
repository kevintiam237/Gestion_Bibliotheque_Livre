using BibliothequeApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gestion_Bibliotheque_Livre.Models
{
    public class DbContextBibliotheque:DbContext
    {
        public DbContextBibliotheque()
        {
        }
        public DbContextBibliotheque(DbContextOptions<DbContextBibliotheque> options)
            : base(options)
        {
        }

        public DbSet<Auteur> Auteurs { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<DetailsLivre> DetailsLivres { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<LivreCategorie> LivreCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=L-107-MEDDOUR;Database=BibliothequeDB;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-Many: Auteur -> Livres
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.Auteur)
                .WithMany(a => a.Livres)
                .HasForeignKey(l => l.AuteurId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One: Livre -> DetailsLivre
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.DetailsLivre)
                .WithOne(d => d.Livre)
                .HasForeignKey<DetailsLivre>(d => d.LivreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many: Livre <-> Categorie via LivreCategorie
            modelBuilder.Entity<LivreCategorie>()
                .HasKey(lc => new { lc.LivreId, lc.CategorieId });

            modelBuilder.Entity<LivreCategorie>()
                .HasOne(lc => lc.Livre)
                .WithMany(l => l.LivreCategories)
                .HasForeignKey(lc => lc.LivreId);

            modelBuilder.Entity<LivreCategorie>()
                .HasOne(lc => lc.Categorie)
                .WithMany(c => c.LivreCategories)
                .HasForeignKey(lc => lc.CategorieId);

            // Seed data
            modelBuilder.Entity<Categorie>().HasData(
                new Categorie { Id = 1, Nom = "Roman" },
                new Categorie { Id = 2, Nom = "Science-Fiction" },
                new Categorie { Id = 3, Nom = "Fantastique" },
                new Categorie { Id = 4, Nom = "Policier" },
                new Categorie { Id = 5, Nom = "Biographie" },
                new Categorie { Id = 6, Nom = "Histoire" }
            );
        }
    }
}
