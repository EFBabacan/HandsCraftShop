using MediatR;

namespace HandCraft.Siparis.Application.Features.Commands.SiparisDurumGuncelle
{
    // CQRS Command: admin siparis durumunu gunceller (1=Onaylandi, 2=Iptal).
    public class SiparisDurumGuncelleCommand : IRequest<bool>
    {
        public int SiparisId { get; set; }
        public int YeniDurum { get; set; }
        public string? KargoKodu { get; set; }
    }
}
