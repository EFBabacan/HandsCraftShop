namespace HandCraft.Web.Models
{
    // Gateway/servislerin dondurdugu ServisSonuc<T> zarfinin istemci karsiligi.
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

    public class KategoriVm
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
    }

    // --- Yorum ---
    public class YorumVm
    {
        public string KullaniciAdi { get; set; } = string.Empty;
        public int Puan { get; set; }
        public string Metin { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
    }

    public class YorumOzetVm
    {
        public double OrtalamaPuan { get; set; }
        public int YorumSayisi { get; set; }
        public List<YorumVm> Yorumlar { get; set; } = new();
    }

    // Urun detay sayfasi: urun + yorum ozeti birlikte.
    public class UrunDetayVm
    {
        public UrunVm Urun { get; set; } = new();
        public YorumOzetVm Yorumlar { get; set; } = new();
    }

    // --- Profil ---
    public class ProfilVm
    {
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // --- Indirim ---
    public class IndirimVm
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Oran { get; set; }   // yuzde (or. 15 = %15)
        public string Kod { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // --- Sepet ---
    public class SepetVm
    {
        public string UserId { get; set; } = string.Empty;
        public List<SepetItemVm> Urunler { get; set; } = new();
        public decimal ToplamTutar { get; set; }
    }

    public class SepetItemVm
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    // Sepete ekleme istegi govdesi (Sepet API SepeteEkleDto ile ayni alanlar).
    public class SepeteEkleVm
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; } = 1;
        public string ImageUrl { get; set; } = string.Empty;
    }

    // --- Siparis ---
    public class SiparisVm
    {
        public int Id { get; set; }
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

    // Checkout sayfasi view model: sepet + uygulanan indirim.
    public class CheckoutVm
    {
        public SepetVm Sepet { get; set; } = new();
        public string? IndirimKodu { get; set; }
        public decimal IndirimOrani { get; set; }      // %; 0 ise indirim yok
        public string? IndirimMesaji { get; set; }     // "Gecersiz kod" gibi geri bildirim

        public decimal AraToplam => Sepet.ToplamTutar;
        public decimal IndirimTutari => Math.Round(AraToplam * IndirimOrani / 100m, 2);
        public decimal GenelToplam => AraToplam - IndirimTutari;
    }

    // Checkout/odeme istegi govdesi (Odeme API OdemeAlDto ile ayni alanlar).
    public class OdemeAlVm
    {
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;
        public string KartSahibi { get; set; } = string.Empty;
        public string KartNo { get; set; } = string.Empty;
        public List<OdemeUrunVm> Urunler { get; set; } = new();
    }

    public class OdemeUrunVm
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
