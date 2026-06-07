using HandCraft.Siparis.Domain.Common;

namespace HandCraft.Siparis.Domain.Entities
{
    // Ana siparis kayit (aggregate root). MSSQL'de 'Siparisler' tablosu.
    public class Siparis : CreationalEntity<int>
    {
        public string UserId { get; set; } = string.Empty;

        public decimal ToplamTutar { get; set; }

        // Onay/kargo durumu (yeni siparis Beklemede gelir).
        public SiparisDurum Durum { get; set; } = SiparisDurum.Beklemede;

        // Kargo takip kodu (admin Kargoda durumuna gecirirken girer).
        public string? KargoKodu { get; set; }

        // Teslimat adresi (owned/complex tip).
        public Address Adres { get; set; } = new();

        // Siparis kalemleri.
        public List<SiparisUrunBilgi> Urunler { get; set; } = new();
    }
}
