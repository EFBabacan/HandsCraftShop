using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Katalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrunController : ControllerBase
    {
        private readonly IUrunService _urunService;

        public UrunController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        // Okuma -> public
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Hepsi()
        {
            var sonuc = await _urunService.HepsiniGetirAsync();
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpGet("kategori/{kategoriId}")]
        [AllowAnonymous]
        public async Task<IActionResult> KategoriyeGore(string kategoriId)
        {
            var sonuc = await _urunService.KategoriyeGoreGetirAsync(kategoriId);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Getir(string id)
        {
            var sonuc = await _urunService.IdileGetirAsync(id);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Yazma -> admin rolu
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Ekle([FromBody] UrunCreateDto dto)
        {
            var sonuc = await _urunService.EkleAsync(dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Guncelle([FromBody] UrunUpdateDto dto)
        {
            var sonuc = await _urunService.GuncelleAsync(dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Sil(string id)
        {
            var sonuc = await _urunService.SilAsync(id);
            return StatusCode(sonuc.StatusCode, sonuc);
        }
    }
}
