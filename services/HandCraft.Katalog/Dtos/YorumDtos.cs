namespace HandCraft.Katalog.Dtos
{
    // Disari donen yorum gorunumu.
    public class YorumViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string UrunId { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public int Puan { get; set; }
        public string Metin { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
    }

    // Yorum ekleme istegi (UserId/KullaniciAdi token'dan alinir).
    public class YorumCreateDto
    {
        public string UrunId { get; set; } = string.Empty;
        public int Puan { get; set; }
        public string Metin { get; set; } = string.Empty;
    }

    // Bir urunun yorum ozeti: ortalama puan + adet + liste.
    public class YorumOzetDto
    {
        public double OrtalamaPuan { get; set; }
        public int YorumSayisi { get; set; }
        public List<YorumViewDto> Yorumlar { get; set; } = new();
    }
}
