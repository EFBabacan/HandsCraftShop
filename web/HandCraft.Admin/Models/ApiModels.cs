namespace HandCraft.Admin.Models
{
    public class ApiSonuc<T>
    {
        public T? Data { get; set; }
        public bool BasariliMi { get; set; }
        public int StatusCode { get; set; }
    }

    // --- Katalog ---
    public class UrunVm
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string KategoriId { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stok { get; set; }
    }

    // Urun olusturma/guncelleme formu (Katalog UrunCreate/UpdateDto ile uyumlu)
    public class UrunFormVm
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string KategoriId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stok { get; set; }
    }

    public class KategoriVm
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
    }

    // --- Kullanici (Keycloak) ---
    public class KullaniciVm
    {
        public string Id { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public bool Aktif { get; set; }
        public List<string> Roller { get; set; } = new();
    }

    public class KullaniciFormVm
    {
        public string Id { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Parola { get; set; }
        public bool Aktif { get; set; } = true;
        public List<string> Roller { get; set; } = new();
    }

    // --- Indirim ---
    public class IndirimVm
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Oran { get; set; }
        public string Kod { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // --- Siparis (listeleme) ---
    public class SiparisVm
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal ToplamTutar { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public int Durum { get; set; }
        public string DurumMetni { get; set; } = string.Empty;
        public string? KargoKodu { get; set; }
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;
        public List<SiparisUrunVm> Urunler { get; set; } = new();
    }

    public class SiparisUrunVm
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
