using HandCraft.Odeme.Dtos;
using HandCraft.Ortak.Identity;
using HandCraft.Ortak.Mesajlar;
using HandCraft.Ortak.Sonuc;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Odeme.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "customer")]
    public class OdemeController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IIdentityHelperService _identity;

        // Bolum 4 madde 11: kuyruk adi consumer ile birebir eslesmeli.
        private const string KuyrukUri = "queue:siparis-olustur-service";

        public OdemeController(ISendEndpointProvider sendEndpointProvider, IIdentityHelperService identity)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _identity = identity;
        }

        // Sahte odeme -> her zaman basarili -> siparis olustur mesajini RabbitMQ'ya gonder.
        [HttpPost]
        public async Task<IActionResult> OdemeAl([FromBody] OdemeAlDto dto)
        {
            if (dto.Urunler is null || dto.Urunler.Count == 0)
                return BadRequest(ServisSonuc.Hata("Sepet bos; odeme alinamaz."));

            var userId = _identity.GetUserId();

            // (Demo) sahte odeme islemi burada basarili kabul edilir.

            var mesaj = new SiparisOlusturMessageCommand
            {
                Siparis = new SiparisDto
                {
                    UserId = userId,
                    Il = dto.Il,
                    Ilce = dto.Ilce,
                    AcikAdres = dto.AcikAdres,
                    ToplamTutar = dto.Urunler.Sum(u => u.Fiyat * u.Adet),
                    Urunler = dto.Urunler.Select(u => new SiparisUrunBilgiDto
                    {
                        UrunId = u.UrunId,
                        UrunAdi = u.UrunAdi,
                        Fiyat = u.Fiyat,
                        Adet = u.Adet
                    }).ToList()
                }
            };

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(KuyrukUri));
            await endpoint.Send(mesaj);

            return Ok(ServisSonuc.Basarili());
        }
    }
}
