using AutoMapper;
using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Models;

namespace HandCraft.Katalog.Mapping
{
    // Model <-> DTO eslemeleri (Bolum 4 madde 8).
    public class KatalogMappingProfile : Profile
    {
        public KatalogMappingProfile()
        {
            // Kategori
            CreateMap<Kategori, KategoriViewDto>();
            CreateMap<KategoriCreateDto, Kategori>()
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<KategoriUpdateDto, Kategori>();

            // Urun
            // KategoriAdi Mongo'da tutulmaz; servis katmaninda doldurulur -> Ignore.
            CreateMap<Urun, UrunViewDto>()
                .ForMember(d => d.KategoriAdi, o => o.Ignore());
            CreateMap<UrunCreateDto, Urun>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.EklenmeTarihi, o => o.Ignore());
            CreateMap<UrunUpdateDto, Urun>()
                .ForMember(d => d.EklenmeTarihi, o => o.Ignore());

            // Yorum
            CreateMap<Yorum, YorumViewDto>();
        }
    }
}
