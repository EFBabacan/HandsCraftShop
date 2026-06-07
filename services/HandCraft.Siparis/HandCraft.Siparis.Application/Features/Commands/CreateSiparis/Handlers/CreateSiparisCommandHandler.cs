using HandCraft.Siparis.Domain.Entities;
using HandCraft.Siparis.Persistence.Context;
using MediatR;

namespace HandCraft.Siparis.Application.Features.Commands.CreateSiparis.Handlers
{
    // GERCEKTEN calisir: siparisi MSSQL'e yazar (Bolum 6 - referansta stub'ti).
    public class CreateSiparisCommandHandler : IRequestHandler<CreateSiparisCommand, int>
    {
        private readonly SiparisDbContext _context;

        public CreateSiparisCommandHandler(SiparisDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateSiparisCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Siparis;

            var siparis = new Domain.Entities.Siparis
            {
                UserId = request.UserId,
                OlusturmaTarihi = DateTime.UtcNow,
                Adres = new Address
                {
                    Il = dto.Il,
                    Ilce = dto.Ilce,
                    AcikAdres = dto.AcikAdres
                },
                Urunler = dto.Urunler.Select(u => new SiparisUrunBilgi
                {
                    UrunId = u.UrunId,
                    UrunAdi = u.UrunAdi,
                    Fiyat = u.Fiyat,
                    Adet = u.Adet
                }).ToList()
            };

            siparis.ToplamTutar = siparis.Urunler.Sum(u => u.Fiyat * u.Adet);

            await _context.Siparisler.AddAsync(siparis, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return siparis.Id;
        }
    }
}
