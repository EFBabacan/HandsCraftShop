namespace HandCraft.Siparis.Application.Features.Queries.GetSiparislerByUserId.Dtos
{
    // Kullanicinin siparis listesi gorunumu.
    public class SiparisListDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal ToplamTutar { get; set; }
        public DateTime OlusturmaTarihi { get; set; }

        // Durum kodu + okunabilir metin + kargo takip kodu.
        public int Durum { get; set; }
        public string DurumMetni { get; set; } = string.Empty;
        public string? KargoKodu { get; set; }

        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;

        public List<SiparisUrunListDto> Urunler { get; set; } = new();
    }

    public class SiparisUrunListDto
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
