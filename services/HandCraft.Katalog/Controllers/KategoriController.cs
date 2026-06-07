using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Katalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KategoriController : ControllerBase
    {
        private readonly IKategoriService _kategoriService;

        public KategoriController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        // Okuma -> public
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Hepsi()
        {
            var sonuc = await _kategoriService.HepsiniGetirAsync();
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Getir(string id)
        {
            var sonuc = await _kategoriService.IdileGetirAsync(id);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Yazma -> admin rolu
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Ekle([FromBody] KategoriCreateDto dto)
        {
            var sonuc = await _kategoriService.EkleAsync(dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Guncelle([FromBody] KategoriUpdateDto dto)
        {
            var sonuc = await _kategoriService.GuncelleAsync(dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Sil(string id)
        {
            var sonuc = await _kategoriService.SilAsync(id);
            return StatusCode(sonuc.StatusCode, sonuc);
        }
    }
}
