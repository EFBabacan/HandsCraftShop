using HandCraft.Siparis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HandCraft.Siparis.Persistence.Context
{
    // EF Core DbContext (Bolum 4 madde 13). Migration'lar bu assembly'de.
    public class SiparisDbContext : DbContext
    {
        public SiparisDbContext(DbContextOptions<SiparisDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Siparis> Siparisler => Set<Domain.Entities.Siparis>();
        public DbSet<SiparisUrunBilgi> SiparisUrunBilgileri => Set<SiparisUrunBilgi>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain.Entities.Siparis>(b =>
            {
                b.ToTable("Siparisler");
                b.HasKey(x => x.Id);
                b.Property(x => x.UserId).IsRequired().HasMaxLength(100);
                b.Property(x => x.ToplamTutar).HasColumnType("decimal(18,2)");
                b.Property(x => x.Durum).HasConversion<int>(); // enum -> int
                b.Property(x => x.KargoKodu).HasMaxLength(100);

                // Address owned tip -> ayni tabloda kolon olarak.
                b.OwnsOne(x => x.Adres, a =>
                {
                    a.Property(p => p.Il).HasColumnName("Il").HasMaxLength(100);
                    a.Property(p => p.Ilce).HasColumnName("Ilce").HasMaxLength(100);
                    a.Property(p => p.AcikAdres).HasColumnName("AcikAdres").HasMaxLength(500);
                });

                // Bir siparisin cok urun kalemi.
                b.HasMany(x => x.Urunler)
                 .WithOne()
                 .HasForeignKey(u => u.SiparisId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SiparisUrunBilgi>(b =>
            {
                b.ToTable("SiparisUrunBilgileri");
                b.HasKey(x => x.Id);
                b.Property(x => x.UrunId).IsRequired().HasMaxLength(100);
                b.Property(x => x.UrunAdi).HasMaxLength(250);
                b.Property(x => x.Fiyat).HasColumnType("decimal(18,2)");
            });
        }
    }
}
