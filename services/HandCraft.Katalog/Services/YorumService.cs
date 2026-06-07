using AutoMapper;
using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Models;
using HandCraft.Katalog.Settings;
using HandCraft.Ortak.Sonuc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HandCraft.Katalog.Services
{
    public class YorumService : IYorumService
    {
        private readonly IMongoCollection<Yorum> _yorumlar;
        private readonly IMongoCollection<Urun> _urunler;
        private readonly IMapper _mapper;

        public YorumService(IOptions<MongoDbSettings> ayarlar, IMapper mapper)
        {
            var client = new MongoClient(ayarlar.Value.ConnectionString);
            var db = client.GetDatabase(ayarlar.Value.DatabaseName);
            _yorumlar = db.GetCollection<Yorum>(MongoDbTables.Yorum);
            _urunler = db.GetCollection<Urun>(MongoDbTables.Urun);
            _mapper = mapper;
        }

        public async Task<ServisSonuc<YorumOzetDto>> UrunYorumlariGetirAsync(string urunId)
        {
            var liste = await _yorumlar.Find(y => y.UrunId == urunId)
                .SortByDescending(y => y.Tarih).ToListAsync();

            var dtolar = _mapper.Map<List<YorumViewDto>>(liste);
            var ozet = new YorumOzetDto
            {
                Yorumlar = dtolar,
                YorumSayisi = dtolar.Count,
                OrtalamaPuan = dtolar.Count > 0 ? Math.Round(dtolar.Average(d => d.Puan), 1) : 0
            };
            return ServisSonuc<YorumOzetDto>.Basarili(ozet);
        }

        public async Task<ServisSonuc<YorumViewDto>> EkleAsync(string userId, string kullaniciAdi, YorumCreateDto dto)
        {
            if (dto.Puan < 1 || dto.Puan > 5)
                return ServisSonuc<YorumViewDto>.Hata("Puan 1-5 arasinda olmali.", 400);

            var urunVar = await _urunler.Find(u => u.Id == dto.UrunId).AnyAsync();
            if (!urunVar)
                return ServisSonuc<YorumViewDto>.Hata("Urun bulunamadi.", 404);

            var yorum = new Yorum
            {
                UrunId = dto.UrunId,
                UserId = userId,
                KullaniciAdi = string.IsNullOrWhiteSpace(kullaniciAdi) ? "Anonim" : kullaniciAdi,
                Puan = dto.Puan,
                Metin = dto.Metin,
                Tarih = DateTime.UtcNow
            };
            await _yorumlar.InsertOneAsync(yorum);
            return ServisSonuc<YorumViewDto>.Basarili(_mapper.Map<YorumViewDto>(yorum), 201);
        }
    }
}
