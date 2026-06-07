using HandCraft.Indirim.Dtos;
using HandCraft.Ortak.Sonuc;

namespace HandCraft.Indirim.Services
{
    public interface IMyIndirimService
    {
        Task TabloyuHazirlaAsync();
        Task<ServisSonuc<List<IndirimDto>>> HepsiniGetirAsync();
        Task<ServisSonuc<IndirimDto>> IdileGetirAsync(int id);
        Task<ServisSonuc<IndirimDto>> KodIleGetirAsync(string kod);
        Task<ServisSonuc<IndirimDto>> EkleAsync(IndirimCreateDto dto);
        Task<ServisSonuc> GuncelleAsync(IndirimUpdateDto dto);
        Task<ServisSonuc> SilAsync(int id);
    }
}
