using HandCraft.Katalog.Models;
using HandCraft.Katalog.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HandCraft.Katalog.Services
{
    // Uygulama acilisinda koleksiyonlar bossa ornek el-isi verisi yukler.
    public class KatalogSeeder
    {
        private readonly IMongoCollection<Urun> _urunler;
        private readonly IMongoCollection<Kategori> _kategoriler;

        public KatalogSeeder(IOptions<MongoDbSettings> ayarlar)
        {
            var client = new MongoClient(ayarlar.Value.ConnectionString);
            var db = client.GetDatabase(ayarlar.Value.DatabaseName);
            _urunler = db.GetCollection<Urun>(MongoDbTables.Urun);
            _kategoriler = db.GetCollection<Kategori>(MongoDbTables.Kategori);
        }

        public async Task SeedAsync()
        {
            if (await _kategoriler.Find(_ => true).AnyAsync())
                return; // zaten seed edilmis

            var seramik = new Kategori { Ad = "Seramik" };
            var ahsap = new Kategori { Ad = "Ahsap" };
            var taki = new Kategori { Ad = "Taki" };
            var orgu = new Kategori { Ad = "Orgu" };

            await _kategoriler.InsertManyAsync(new[] { seramik, ahsap, taki, orgu });

            var simdi = DateTime.UtcNow;
            var urunler = new List<Urun>
            {
                new() { Ad = "El Yapimi Seramik Kupa", Aciklama = "Sirli, mavi desenli kahve kupasi.", Fiyat = 180m, KategoriId = seramik.Id, Stok = 25, ImageUrl = "https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "Seramik Sunum Tabagi", Aciklama = "Elde sekillendirilmis dekoratif tabak.", Fiyat = 320m, KategoriId = seramik.Id, Stok = 12, ImageUrl = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "Seramik Saksilik", Aciklama = "Mini sukulent saksisi, mat bitis.", Fiyat = 140m, KategoriId = seramik.Id, Stok = 30, ImageUrl = "https://images.unsplash.com/photo-1485955900006-10f4d324d411?w=600&q=80", EklenmeTarihi = simdi },

                new() { Ad = "Ahsap Kesim Tahtasi", Aciklama = "Ceviz agacindan, dogal yag korumali.", Fiyat = 260m, KategoriId = ahsap.Id, Stok = 18, ImageUrl = "https://images.unsplash.com/photo-1593618998160-e34014e67546?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "El Oyma Ahsap Kasik Seti", Aciklama = "3'lu zeytin agaci kasik seti.", Fiyat = 210m, KategoriId = ahsap.Id, Stok = 22, ImageUrl = "https://images.unsplash.com/photo-1591814468924-caf88d1232e1?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "Ahsap Duvar Saati", Aciklama = "Minimalist mese duvar saati.", Fiyat = 480m, KategoriId = ahsap.Id, Stok = 8, ImageUrl = "https://images.unsplash.com/photo-1563861826100-9cb868fdbe1c?w=600&q=80", EklenmeTarihi = simdi },

                new() { Ad = "Gumus Telkari Kolye", Aciklama = "El isi telkari, 925 ayar gumus.", Fiyat = 750m, KategoriId = taki.Id, Stok = 6, ImageUrl = "https://images.unsplash.com/photo-1599643478518-a784e5dc4c8f?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "Dogal Tas Bileklik", Aciklama = "Ametist ve akik tas bileklik.", Fiyat = 190m, KategoriId = taki.Id, Stok = 35, ImageUrl = "https://images.unsplash.com/photo-1611591437281-460bfbe1220a?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "El Yapimi Cam Boncuk Kupe", Aciklama = "Renkli fume cam boncuk kupe.", Fiyat = 130m, KategoriId = taki.Id, Stok = 40, ImageUrl = "https://images.unsplash.com/photo-1535632066927-ab7c9ab60908?w=600&q=80", EklenmeTarihi = simdi },

                new() { Ad = "Orgu Bebek Battaniyesi", Aciklama = "Yumusak pamuk iplikten, hipoalerjenik.", Fiyat = 420m, KategoriId = orgu.Id, Stok = 10, ImageUrl = "https://images.unsplash.com/photo-1522771930-78848d9293e8?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "El Orgusu Atki", Aciklama = "Yun karisimi, kis icin sicak atki.", Fiyat = 230m, KategoriId = orgu.Id, Stok = 20, ImageUrl = "https://images.unsplash.com/photo-1520903920243-00d872a2d1c9?w=600&q=80", EklenmeTarihi = simdi },
                new() { Ad = "Amigurumi Pelus Oyuncak", Aciklama = "El orgusu sevimli tavsan figuru.", Fiyat = 175m, KategoriId = orgu.Id, Stok = 15, ImageUrl = "https://images.unsplash.com/photo-1558877385-81a1c7e67d72?w=600&q=80", EklenmeTarihi = simdi },
            };

            await _urunler.InsertManyAsync(urunler);
        }
    }
}
