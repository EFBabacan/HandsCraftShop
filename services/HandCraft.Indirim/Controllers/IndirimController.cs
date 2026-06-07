using HandCraft.Indirim.Dtos;
using HandCraft.Indirim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Indirim.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndirimController : ControllerBase
    {
        private readonly IMyIndirimService _indirimService;

        public IndirimController(IMyIndirimService indirimService)
        {
            _indirimService = indirimService;
        }

        // Tum indirimleri listele -> admin (yonetim).
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Hepsi()
        {
            var sonuc = await _indirimService.HepsiniGetirAsync();
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Getir(int id)
        {
            var sonuc = await _indirimService.IdileGetirAsync(id);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Kod ile aktif indirim getir -> giris yapmis kullanici (musteri sepette uygular).
        [HttpGet("kod/{kod}")]
        [Authorize]
        public async Task<IActionResult> KodIle(string kod)
        {
            var sonuc = await _indirimService.KodIleGetirAsync(kod);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Ekle([FromBody] IndirimCreateDto dto)
        {
            var sonuc = await _indirimService.EkleAsync(dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Guncelle([FromBody] IndirimUpdateDto dto)
        {
            var sonuc = await _indirimService.GuncelleAsync(dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var sonuc = await _indirimService.SilAsync(id);
            return StatusCode(sonuc.StatusCode, sonuc);
        }
    }
}
