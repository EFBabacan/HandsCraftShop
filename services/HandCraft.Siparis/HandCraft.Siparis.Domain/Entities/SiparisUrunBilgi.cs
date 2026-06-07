using HandCraft.Siparis.Domain.Common;

namespace HandCraft.Siparis.Domain.Entities
{
    // Siparis kalemi. MSSQL'de 'SiparisUrunBilgileri' tablosu.
    public class SiparisUrunBilgi : BaseEntity<int>
    {
        public int SiparisId { get; set; }

        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
