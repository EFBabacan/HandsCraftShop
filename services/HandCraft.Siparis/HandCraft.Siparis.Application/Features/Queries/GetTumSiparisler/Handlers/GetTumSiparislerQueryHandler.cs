using HandCraft.Siparis.Application.Features.Queries.GetSiparislerByUserId.Dtos;
using HandCraft.Siparis.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HandCraft.Siparis.Application.Features.Queries.GetTumSiparisler.Handlers
{
    public class GetTumSiparislerQueryHandler
        : IRequestHandler<GetTumSiparislerQuery, List<SiparisListDto>>
    {
        private readonly SiparisDbContext _context;

        public GetTumSiparislerQueryHandler(SiparisDbContext context)
        {
            _context = context;
        }

        public async Task<List<SiparisListDto>> Handle(
            GetTumSiparislerQuery request, CancellationToken cancellationToken)
        {
            return await _context.Siparisler
                .AsNoTracking()
                .OrderByDescending(s => s.OlusturmaTarihi)
                .Select(s => new SiparisListDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    ToplamTutar = s.ToplamTutar,
                    OlusturmaTarihi = s.OlusturmaTarihi,
                    Durum = (int)s.Durum,
                    DurumMetni = s.Durum.ToString(),
                    KargoKodu = s.KargoKodu,
                    Il = s.Adres.Il,
                    Ilce = s.Adres.Ilce,
                    AcikAdres = s.Adres.AcikAdres,
                    Urunler = s.Urunler.Select(u => new SiparisUrunListDto
                    {
                        UrunId = u.UrunId,
                        UrunAdi = u.UrunAdi,
                        Fiyat = u.Fiyat,
                        Adet = u.Adet
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
