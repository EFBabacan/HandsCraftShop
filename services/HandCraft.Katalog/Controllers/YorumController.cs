using HandCraft.Katalog.Dtos;
using HandCraft.Katalog.Services;
using HandCraft.Ortak.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Katalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YorumController : ControllerBase
    {
        private readonly IYorumService _yorumService;
        private readonly IIdentityHelperService _identity;

        public YorumController(IYorumService yorumService, IIdentityHelperService identity)
        {
            _yorumService = yorumService;
            _identity = identity;
        }

        // Bir urunun yorumlari + ortalama puan -> public
        [HttpGet("urun/{urunId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UrunYorumlari(string urunId)
        {
            var sonuc = await _yorumService.UrunYorumlariGetirAsync(urunId);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Yorum ekle -> giris yapmis kullanici
        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Ekle([FromBody] YorumCreateDto dto)
        {
            var userId = _identity.GetUserId();
            var kullaniciAdi = _identity.GetUserName();
            var sonuc = await _yorumService.EkleAsync(userId, kullaniciAdi, dto);
            return StatusCode(sonuc.StatusCode, sonuc);
        }
    }
}
