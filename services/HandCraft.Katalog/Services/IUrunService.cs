using HandCraft.Katalog.Dtos;
using HandCraft.Ortak.Sonuc;

namespace HandCraft.Katalog.Services
{
    public interface IUrunService
    {
        Task<ServisSonuc<List<UrunViewDto>>> HepsiniGetirAsync();
        Task<ServisSonuc<List<UrunViewDto>>> KategoriyeGoreGetirAsync(string kategoriId);
        Task<ServisSonuc<UrunViewDto>> IdileGetirAsync(string id);
        Task<ServisSonuc<UrunViewDto>> EkleAsync(UrunCreateDto dto);
        Task<ServisSonuc> GuncelleAsync(UrunUpdateDto dto);
        Task<ServisSonuc> SilAsync(string id);
    }
}
