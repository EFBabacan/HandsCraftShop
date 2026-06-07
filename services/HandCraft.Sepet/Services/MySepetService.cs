using System.Text.Json;
using HandCraft.Sepet.Dtos;
using HandCraft.Ortak.Sonuc;
using StackExchange.Redis;

namespace HandCraft.Sepet.Services
{
    // Redis tabanli sepet servisi. Anahtar: sepet:{userId} (Bolum 7/Faz 4).
    public class MySepetService : ISepetService
    {
        private readonly IDatabase _db;

        public MySepetService(RedisService redisService)
        {
            _db = redisService.GetDatabase();
        }

        private static string Anahtar(string userId) => $"sepet:{userId}";

        private async Task<SepetDto> SepetiOkuAsync(string userId)
        {
            var json = await _db.StringGetAsync(Anahtar(userId));
            if (json.IsNullOrEmpty)
                return new SepetDto { UserId = userId };

            return JsonSerializer.Deserialize<SepetDto>(json!) ?? new SepetDto { UserId = userId };
        }

        private async Task SepetiYazAsync(SepetDto sepet)
        {
            var json = JsonSerializer.Serialize(sepet);
            await _db.StringSetAsync(Anahtar(sepet.UserId), json);
        }

        public async Task<ServisSonuc<SepetDto>> GetirAsync(string userId)
        {
            var sepet = await SepetiOkuAsync(userId);
            return ServisSonuc<SepetDto>.Basarili(sepet);
        }

        public async Task<ServisSonuc<SepetDto>> EkleAsync(string userId, SepeteEkleDto item)
        {
            if (string.IsNullOrWhiteSpace(item.UrunId))
                return ServisSonuc<SepetDto>.Hata("UrunId zorunlu.", 400);
            if (item.Adet <= 0)
                return ServisSonuc<SepetDto>.Hata("Adet 0'dan buyuk olmali.", 400);

            var sepet = await SepetiOkuAsync(userId);
            var mevcut = sepet.Urunler.FirstOrDefault(u => u.UrunId == item.UrunId);
            if (mevcut is not null)
            {
                // Ayni urun zaten varsa adedi artir.
                mevcut.Adet += item.Adet;
                mevcut.Fiyat = item.Fiyat; // guncel fiyati al
            }
            else
            {
                sepet.Urunler.Add(new SepetItemDto
                {
                    UrunId = item.UrunId,
                    UrunAdi = item.UrunAdi,
                    Fiyat = item.Fiyat,
                    Adet = item.Adet,
                    ImageUrl = item.ImageUrl
                });
            }

            await SepetiYazAsync(sepet);
            return ServisSonuc<SepetDto>.Basarili(sepet);
        }

        public async Task<ServisSonuc<SepetDto>> UrunSilAsync(string userId, string urunId)
        {
            var sepet = await SepetiOkuAsync(userId);
            var silinen = sepet.Urunler.RemoveAll(u => u.UrunId == urunId);
            if (silinen == 0)
                return ServisSonuc<SepetDto>.Hata("Urun sepette bulunamadi.", 404);

            await SepetiYazAsync(sepet);
            return ServisSonuc<SepetDto>.Basarili(sepet);
        }

        public async Task<ServisSonuc> TemizleAsync(string userId)
        {
            await _db.KeyDeleteAsync(Anahtar(userId));
            return ServisSonuc.Basarili();
        }
    }
}
