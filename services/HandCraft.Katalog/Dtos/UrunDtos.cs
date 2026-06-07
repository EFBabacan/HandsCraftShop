namespace HandCraft.Katalog.Dtos
{
    // Disari donen urun gorunumu (kategori adi join'lenmis halde).
    public class UrunViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string KategoriId { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stok { get; set; }
        public DateTime EklenmeTarihi { get; set; }
    }

    // Urun olusturma istegi.
    public class UrunCreateDto
    {
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string KategoriId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stok { get; set; }
    }

    // Urun guncelleme istegi.
    public class UrunUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string KategoriId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stok { get; set; }
    }
}
