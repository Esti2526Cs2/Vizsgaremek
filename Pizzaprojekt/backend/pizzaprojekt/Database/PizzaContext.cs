using Microsoft.EntityFrameworkCore; //Az EF Core specifikus anotációk használatához
using pizzaprojekt.Models; //A modell osztályok használatához
using System; //Alapvető rendszerkönyvtárak
using System.Collections.Generic; //Gyűjtemények kezeléséhez
using System.Linq; //LINQ lekérdezésekhez
using System.Text; //Szövegkezeléshez
using System.Threading.Tasks; //Aszinkron programozáshoz
namespace pizzaprojekt.Database //Adatbázis kontextus definiálása az Entity Framework Core használatával
{
    public class PizzaContext : DbContext //Adatbázis kontextus az Entity Framework Core használatával
    {
        public DbSet<Feltetek> Feltetek { get; set; } //Dbset elem a Feltetek tábla kezeléséhez
        public DbSet<Keri> Keri { get; set; }  //dbset elem a Keri tábla kezeléséhez
        public DbSet<Kiszallitasi_Hely> Kiszallitasi_Hely { get; set; } //dbset elem a KiszalitasiHely tábla kezeléséhez
        public DbSet<Lead> Lead { get; set; } // dbset elem a Lead tábla kezeléséhez
        public DbSet<Pizza> Pizza { get; set; } //dbset elem a Pizza tábla kezeléséhez
        public DbSet<Pizza_Feltetek> Pizza_Feltetek { get; set; } //dbset elem a PizzaFeltetek tábla kezeléséhez
        public DbSet<Rendeles> Rendeles { get; set; } //dbset elem a Rendeles tábla kezeléséhez
        public DbSet<Tartalmaz> Tartalmaz { get; set; } //dbset elem a Tartalmaz tábla kezeléséhez
        public DbSet<Vasarlo> Vasarlo { get; set; } //dbset elem a Vasarlo tábla kezeléséhez

        public PizzaContext(DbContextOptions<PizzaContext> options) : base(options) { } //adatbázis kapcsolat konfigurációja a konstruktorban meghívott opciók alapján

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vasarlo>(entity =>
            {
                entity.HasKey(v => v.RendeloAzonosito);
                entity.Property(v => v.RendeloAzonosito).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Kiszallitasi_Hely>(entity =>
            {
                entity.HasKey(k => k.CimId);
                entity.Property(k => k.CimId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Rendeles>(entity =>
            {
                entity.HasKey(r => r.RendelesId);
                entity.Property(r => r.RendelesId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Keri>(entity =>
            {
                entity.HasKey(k => new { k.CimId, k.RendeloAzonosito });
                entity.HasOne(k => k.KiszalitasiHely)
                    .WithMany()
                    .HasForeignKey(k => k.CimId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(k => k.Vasarlo)
                    .WithMany()
                    .HasForeignKey(k => k.RendeloAzonosito)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Lead>(entity =>
            {
                entity.HasKey(l => new { l.RendeloAzonosito, l.RendelesiId });
                entity.HasOne(l => l.Vasarlo)
                    .WithMany()
                    .HasForeignKey(l => l.RendeloAzonosito)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(l => l.Rendeles)
                    .WithMany()
                    .HasForeignKey(l => l.RendelesiId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tartalmaz>(entity =>
            {
                entity.HasKey(t => new { t.RendelesId, t.PizzaId });
                entity.HasOne(t => t.Rendeles)
                    .WithMany()
                    .HasForeignKey(t => t.RendelesId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(t => t.Pizza)
                    .WithMany()
                    .HasForeignKey(t => t.PizzaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
       

}
    
