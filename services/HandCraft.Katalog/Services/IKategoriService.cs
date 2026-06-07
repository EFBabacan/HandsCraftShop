using HandCraft.Katalog.Dtos;
using HandCraft.Ortak.Sonuc;

namespace HandCraft.Katalog.Services
{
    public interface IKategoriService
    {
        Task<ServisSonuc<List<KategoriViewDto>>> HepsiniGetirAsync();
        Task<ServisSonuc<KategoriViewDto>> IdileGetirAsync(string id);
        Task<ServisSonuc<KategoriViewDto>> EkleAsync(KategoriCreateDto dto);
        Task<ServisSonuc> GuncelleAsync(KategoriUpdateDto dto);
        Task<ServisSonuc> SilAsync(string id);
    }
}
