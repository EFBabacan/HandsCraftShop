namespace HandCraft.Odeme.Dtos
{
    // Odeme alma istegi. UserId token'dan alinir, body'de gelmez.
    public class OdemeAlDto
    {
        // Teslimat adresi
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;

        // Sahte kart bilgisi (dogrulanmaz, demo)
        public string KartSahibi { get; set; } = string.Empty;
        public string KartNo { get; set; } = string.Empty;

        public List<OdemeUrunDto> Urunler { get; set; } = new();
    }

    public class OdemeUrunDto
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
