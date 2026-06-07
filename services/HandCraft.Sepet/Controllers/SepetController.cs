using HandCraft.Ortak.Identity;
using HandCraft.Ortak.Sonuc;
using HandCraft.Sepet.Dtos;
using HandCraft.Sepet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Sepet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "customer")] // sepet musteriye ait
    public class SepetController : ControllerBase
    {
        private readonly ISepetService _sepetService;
        private readonly IIdentityHelperService _identity;

        public SepetController(ISepetService sepetService, IIdentityHelperService identity)
        {
            _sepetService = sepetService;
            _identity = identity;
        }

        // Sepetimi getir
        [HttpGet]
        public async Task<IActionResult> Getir()
        {
            var userId = _identity.GetUserId();
            var sonuc = await _sepetService.GetirAsync(userId);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Sepete urun ekle (UserId token'dan, body'de degil)
        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] SepeteEkleDto item)
        {
            var userId = _identity.GetUserId();
            var sonuc = await _sepetService.EkleAsync(userId, item);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Sepetten urun cikar
        [HttpDelete("{urunId}")]
        public async Task<IActionResult> UrunSil(string urunId)
        {
            var userId = _identity.GetUserId();
            var sonuc = await _sepetService.UrunSilAsync(userId, urunId);
            return StatusCode(sonuc.StatusCode, sonuc);
        }

        // Sepeti tamamen temizle
        [HttpDelete]
        public async Task<IActionResult> Temizle()
        {
            var userId = _identity.GetUserId();
            var sonuc = await _sepetService.TemizleAsync(userId);
            return StatusCode(sonuc.StatusCode, sonuc);
        }
    }
}
