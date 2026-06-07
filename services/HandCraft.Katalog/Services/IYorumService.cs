using HandCraft.Katalog.Dtos;
using HandCraft.Ortak.Sonuc;

namespace HandCraft.Katalog.Services
{
    public interface IYorumService
    {
        Task<ServisSonuc<YorumOzetDto>> UrunYorumlariGetirAsync(string urunId);
        Task<ServisSonuc<YorumViewDto>> EkleAsync(string userId, string kullaniciAdi, YorumCreateDto dto);
    }
}
