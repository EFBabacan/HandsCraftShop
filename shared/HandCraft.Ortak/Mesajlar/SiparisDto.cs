namespace HandCraft.Ortak.Mesajlar
{
    // Mesaj/akis icinde tasinan siparis verisi (Odeme -> Siparis).
    public class SiparisDto
    {
        public string UserId { get; set; } = string.Empty;

        // Teslimat adresi
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;

        public List<SiparisUrunBilgiDto> Urunler { get; set; } = new();

        public decimal ToplamTutar { get; set; }
    }

    public class SiparisUrunBilgiDto
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
