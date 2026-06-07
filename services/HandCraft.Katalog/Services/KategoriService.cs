using AutoMapper;
using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Models;
using HandCraft.Katalog.Settings;
using HandCraft.Ortak.Sonuc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HandCraft.Katalog.Services
{
    public class KategoriService : IKategoriService
    {
        private readonly IMongoCollection<Kategori> _kategoriler;
        private readonly IMongoCollection<Urun> _urunler;
        private readonly IMapper _mapper;

        public KategoriService(IOptions<MongoDbSettings> ayarlar, IMapper mapper)
        {
            var client = new MongoClient(ayarlar.Value.ConnectionString);
            var db = client.GetDatabase(ayarlar.Value.DatabaseName);
            _kategoriler = db.GetCollection<Kategori>(MongoDbTables.Kategori);
            _urunler = db.GetCollection<Urun>(MongoDbTables.Urun);
            _mapper = mapper;
        }

        public async Task<ServisSonuc<List<KategoriViewDto>>> HepsiniGetirAsync()
        {
            var liste = await _kategoriler.Find(_ => true).ToListAsync();
            return ServisSonuc<List<KategoriViewDto>>.Basarili(
                _mapper.Map<List<KategoriViewDto>>(liste));
        }

        public async Task<ServisSonuc<KategoriViewDto>> IdileGetirAsync(string id)
        {
            var kategori = await _kategoriler.Find(k => k.Id == id).FirstOrDefaultAsync();
            if (kategori is null)
                return ServisSonuc<KategoriViewDto>.Hata("Kategori bulunamadi.", 404);

            return ServisSonuc<KategoriViewDto>.Basarili(_mapper.Map<KategoriViewDto>(kategori));
        }

        public async Task<ServisSonuc<KategoriViewDto>> EkleAsync(KategoriCreateDto dto)
        {
            var kategori = _mapper.Map<Kategori>(dto);
            await _kategoriler.InsertOneAsync(kategori);
            return ServisSonuc<KategoriViewDto>.Basarili(_mapper.Map<KategoriViewDto>(kategori), 201);
        }

        public async Task<ServisSonuc> GuncelleAsync(KategoriUpdateDto dto)
        {
            var kategori = _mapper.Map<Kategori>(dto);
            var sonuc = await _kategoriler.ReplaceOneAsync(k => k.Id == dto.Id, kategori);
            if (sonuc.MatchedCount == 0)
                return ServisSonuc.Hata("Guncellenecek kategori bulunamadi.", 404);

            return ServisSonuc.Basarili();
        }

        public async Task<ServisSonuc> SilAsync(string id)
        {
            // Kategoriye bagli urun varsa silmeyi engelle.
            var bagliUrunVar = await _urunler.Find(u => u.KategoriId == id).AnyAsync();
            if (bagliUrunVar)
                return ServisSonuc.Hata("Bu kategoriye bagli urunler var; once onlari tasiyin/silin.", 400);

            var sonuc = await _kategoriler.DeleteOneAsync(k => k.Id == id);
            if (sonuc.DeletedCount == 0)
                return ServisSonuc.Hata("Silinecek kategori bulunamadi.", 404);

            return ServisSonuc.Basarili();
        }
    }
}
