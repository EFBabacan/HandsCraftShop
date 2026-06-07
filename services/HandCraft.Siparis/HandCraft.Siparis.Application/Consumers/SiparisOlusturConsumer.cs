using HandCraft.Ortak.Mesajlar;
using HandCraft.Siparis.Application.Features.Commands.CreateSiparis;
using HandCraft.Siparis.Application.Features.Commands.CreateSiparis.Dtos;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HandCraft.Siparis.Application.Consumers
{
    // Odeme servisinin queue:siparis-olustur-service'e gonderdigi mesaji tuketir,
    // MediatR CreateSiparisCommand'e cevirip siparisi MSSQL'e kaydeder (Bolum 6).
    public class SiparisOlusturConsumer : IConsumer<SiparisOlusturMessageCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SiparisOlusturConsumer> _logger;

        public SiparisOlusturConsumer(IMediator mediator, ILogger<SiparisOlusturConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SiparisOlusturMessageCommand> context)
        {
            var mesaj = context.Message.Siparis;
            _logger.LogInformation("SiparisOlusturMessageCommand alindi. UserId={UserId}", mesaj.UserId);

            var command = new CreateSiparisCommand
            {
                UserId = mesaj.UserId,
                Siparis = new CreateSiparisRequestDto
                {
                    Il = mesaj.Il,
                    Ilce = mesaj.Ilce,
                    AcikAdres = mesaj.AcikAdres,
                    Urunler = mesaj.Urunler.Select(u => new CreateSiparisUrunDto
                    {
                        UrunId = u.UrunId,
                        UrunAdi = u.UrunAdi,
                        Fiyat = u.Fiyat,
                        Adet = u.Adet
                    }).ToList()
                }
            };

            var siparisId = await _mediator.Send(command);
            _logger.LogInformation("Siparis kaydedildi. SiparisId={SiparisId}", siparisId);
        }
    }
}
