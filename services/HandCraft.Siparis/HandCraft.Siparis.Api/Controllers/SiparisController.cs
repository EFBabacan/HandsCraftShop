using HandCraft.Ortak.Identity;
using HandCraft.Ortak.Sonuc;
using HandCraft.Siparis.Application.Features.Commands.CreateSiparis;
using HandCraft.Siparis.Application.Features.Commands.CreateSiparis.Dtos;
using HandCraft.Siparis.Application.Features.Commands.SiparisDurumGuncelle;
using HandCraft.Siparis.Application.Features.Queries.GetSiparislerByUserId;
using HandCraft.Siparis.Application.Features.Queries.GetTumSiparisler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Siparis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiparisController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityHelperService _identity;

        public SiparisController(IMediator mediator, IIdentityHelperService identity)
        {
            _mediator = mediator;
            _identity = identity;
        }

        // Kendi siparislerim (musteri)
        [HttpGet]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Siparislerim()
        {
            var userId = _identity.GetUserId();
            var liste = await _mediator.Send(new GetSiparislerByUserIdQuery { UserId = userId });
            return Ok(ServisSonuc<object>.Basarili(liste));
        }

        // Tum siparisler (admin paneli)
        [HttpGet("tumu")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Tumu()
        {
            var liste = await _mediator.Send(new GetTumSiparislerQuery());
            return Ok(ServisSonuc<object>.Basarili(liste));
        }

        // Siparis durumu guncelle (admin). yeniDurum: 0..5 (SiparisDurum). Kargo kodu opsiyonel (query).
        [HttpPut("{id:int}/durum/{yeniDurum:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DurumGuncelle(int id, int yeniDurum, [FromQuery] string? kargoKodu)
        {
            var ok = await _mediator.Send(new SiparisDurumGuncelleCommand
            {
                SiparisId = id,
                YeniDurum = yeniDurum,
                KargoKodu = kargoKodu
            });
            if (!ok) return StatusCode(404, ServisSonuc.Hata("Siparis bulunamadi veya gecersiz durum.", 404));
            return Ok(ServisSonuc.Basarili());
        }

        // Yeni siparis olustur (UserId token'dan)
        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Olustur([FromBody] CreateSiparisRequestDto dto)
        {
            var userId = _identity.GetUserId();
            var siparisId = await _mediator.Send(new CreateSiparisCommand
            {
                UserId = userId,
                Siparis = dto
            });
            return StatusCode(201, ServisSonuc<int>.Basarili(siparisId, 201));
        }
    }
}
