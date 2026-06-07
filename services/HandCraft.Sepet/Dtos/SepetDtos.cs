namespace HandCraft.Sepet.Dtos
{
    // Kullanicinin sepeti. Redis'te 'sepet:{userId}' anahtarinda JSON olarak tutulur.
    public class SepetDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<SepetItemDto> Urunler { get; set; } = new();

        // Sepet toplami (hesaplanir; serialize edilir ama client'tan beklenmez).
        public decimal ToplamTutar => Urunler.Sum(u => u.Fiyat * u.Adet);
    }

    public class SepetItemDto
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    // Sepete urun ekleme istegi (UserId token'dan alinir, body'de gelmez).
    public class SepeteEkleDto
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; } = 1;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
