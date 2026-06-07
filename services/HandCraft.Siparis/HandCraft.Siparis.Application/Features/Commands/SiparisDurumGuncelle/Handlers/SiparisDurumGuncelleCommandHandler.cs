using HandCraft.Siparis.Domain.Entities;
using HandCraft.Siparis.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HandCraft.Siparis.Application.Features.Commands.SiparisDurumGuncelle.Handlers
{
    public class SiparisDurumGuncelleCommandHandler
        : IRequestHandler<SiparisDurumGuncelleCommand, bool>
    {
        private readonly SiparisDbContext _context;

        public SiparisDurumGuncelleCommandHandler(SiparisDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SiparisDurumGuncelleCommand request, CancellationToken cancellationToken)
        {
            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.Id == request.SiparisId, cancellationToken);
            if (siparis is null) return false;

            if (!Enum.IsDefined(typeof(SiparisDurum), request.YeniDurum)) return false;

            siparis.Durum = (SiparisDurum)request.YeniDurum;
            if (!string.IsNullOrWhiteSpace(request.KargoKodu))
                siparis.KargoKodu = request.KargoKodu;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
