using HandCraft.Sepet.Dtos;
using HandCraft.Ortak.Sonuc;

namespace HandCraft.Sepet.Services
{
    public interface ISepetService
    {
        Task<ServisSonuc<SepetDto>> GetirAsync(string userId);
        Task<ServisSonuc<SepetDto>> EkleAsync(string userId, SepeteEkleDto item);
        Task<ServisSonuc<SepetDto>> UrunSilAsync(string userId, string urunId);
        Task<ServisSonuc> TemizleAsync(string userId);
    }
}
