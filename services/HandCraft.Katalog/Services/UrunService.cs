using AutoMapper;
using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Models;
using HandCraft.Katalog.Settings;
using HandCraft.Ortak.Sonuc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HandCraft.Katalog.Services
{
    public class UrunService : IUrunService
    {
        private readonly IMongoCollection<Urun> _urunler;
        private readonly IMongoCollection<Kategori> _kategoriler;
        private readonly IMapper _mapper;

        public UrunService(IOptions<MongoDbSettings> ayarlar, IMapper mapper)
        {
            var client = new MongoClient(ayarlar.Value.ConnectionString);
            var db = client.GetDatabase(ayarlar.Value.DatabaseName);
            _urunler = db.GetCollection<Urun>(MongoDbTables.Urun);
            _kategoriler = db.GetCollection<Kategori>(MongoDbTables.Kategori);
            _mapper = mapper;
        }

        public async Task<ServisSonuc<List<UrunViewDto>>> HepsiniGetirAsync()
        {
            var liste = await _urunler.Find(_ => true).ToListAsync();
            var dtolar = await ZenginlestirAsync(liste);
            return ServisSonuc<List<UrunViewDto>>.Basarili(dtolar);
        }

        public async Task<ServisSonuc<List<UrunViewDto>>> KategoriyeGoreGetirAsync(string kategoriId)
        {
            var liste = await _urunler.Find(u => u.KategoriId == kategoriId).ToListAsync();
            var dtolar = await ZenginlestirAsync(liste);
            return ServisSonuc<List<UrunViewDto>>.Basarili(dtolar);
        }

        public async Task<ServisSonuc<UrunViewDto>> IdileGetirAsync(string id)
        {
            var urun = await _urunler.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (urun is null)
                return ServisSonuc<UrunViewDto>.Hata("Urun bulunamadi.", 404);

            var dto = _mapper.Map<UrunViewDto>(urun);
            dto.KategoriAdi = await KategoriAdiGetirAsync(urun.KategoriId);
            return ServisSonuc<UrunViewDto>.Basarili(dto);
        }

        public async Task<ServisSonuc<UrunViewDto>> EkleAsync(UrunCreateDto dto)
        {
            var kategoriVar = await _kategoriler.Find(k => k.Id == dto.KategoriId).AnyAsync();
            if (!kategoriVar)
                return ServisSonuc<UrunViewDto>.Hata("Belirtilen kategori bulunamadi.", 400);

            var urun = _mapper.Map<Urun>(dto);
            urun.EklenmeTarihi = DateTime.UtcNow;
            await _urunler.InsertOneAsync(urun);

            var view = _mapper.Map<UrunViewDto>(urun);
            view.KategoriAdi = await KategoriAdiGetirAsync(urun.KategoriId);
            return ServisSonuc<UrunViewDto>.Basarili(view, 201);
        }

        public async Task<ServisSonuc> GuncelleAsync(UrunUpdateDto dto)
        {
            var mevcut = await _urunler.Find(u => u.Id == dto.Id).FirstOrDefaultAsync();
            if (mevcut is null)
                return ServisSonuc.Hata("Guncellenecek urun bulunamadi.", 404);

            var kategoriVar = await _kategoriler.Find(k => k.Id == dto.KategoriId).AnyAsync();
            if (!kategoriVar)
                return ServisSonuc.Hata("Belirtilen kategori bulunamadi.", 400);

            var urun = _mapper.Map<Urun>(dto);
            urun.EklenmeTarihi = mevcut.EklenmeTarihi; // ilk eklenme tarihini koru
            await _urunler.ReplaceOneAsync(u => u.Id == dto.Id, urun);
            return ServisSonuc.Basarili();
        }

        public async Task<ServisSonuc> SilAsync(string id)
        {
            var sonuc = await _urunler.DeleteOneAsync(u => u.Id == id);
            if (sonuc.DeletedCount == 0)
                return ServisSonuc.Hata("Silinecek urun bulunamadi.", 404);

            return ServisSonuc.Basarili();
        }

        // ---- yardimcilar ----

        // Urun listesini DTO'ya cevirip her birine KategoriAdi'ni tek seferde doldurur.
        private async Task<List<UrunViewDto>> ZenginlestirAsync(List<Urun> urunler)
        {
            var dtolar = _mapper.Map<List<UrunViewDto>>(urunler);
            if (dtolar.Count == 0)
                return dtolar;

            var kategoriIdler = urunler.Select(u => u.KategoriId).Distinct().ToList();
            var kategoriler = await _kategoriler.Find(k => kategoriIdler.Contains(k.Id)).ToListAsync();
            var harita = kategoriler.ToDictionary(k => k.Id, k => k.Ad);

            foreach (var dto in dtolar)
                dto.KategoriAdi = harita.TryGetValue(dto.KategoriId, out var ad) ? ad : string.Empty;

            return dtolar;
        }

        private async Task<string> KategoriAdiGetirAsync(string kategoriId)
        {
            var kategori = await _kategoriler.Find(k => k.Id == kategoriId).FirstOrDefaultAsync();
            return kategori?.Ad ?? string.Empty;
        }
    }
}
