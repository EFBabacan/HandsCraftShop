using HandCraft.Ortak.Sonuc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Fotograf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FotografController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private static readonly string[] IzinliUzantilar =
            { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        private const long MaxBoyutByte = 5 * 1024 * 1024; // 5 MB

        public FotografController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Multipart gorsel yukleme -> wwwroot/images -> tam URL doner. JWT admin.
        [HttpPost]
        [Authorize(Roles = "admin")]
        [RequestSizeLimit(MaxBoyutByte)]
        public async Task<IActionResult> Yukle(IFormFile dosya)
        {
            if (dosya is null || dosya.Length == 0)
                return BadRequest(ServisSonuc<string>.Hata("Dosya bos."));

            if (dosya.Length > MaxBoyutByte)
                return BadRequest(ServisSonuc<string>.Hata("Dosya 5 MB'tan buyuk olamaz."));

            var uzanti = Path.GetExtension(dosya.FileName).ToLowerInvariant();
            if (!IzinliUzantilar.Contains(uzanti))
                return BadRequest(ServisSonuc<string>.Hata(
                    $"Gecersiz dosya turu. Izinli: {string.Join(", ", IzinliUzantilar)}"));

            // wwwroot bos olabilir -> ContentRootPath altinda garanti dizin.
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var hedefKlasor = Path.Combine(webRoot, "images");
            Directory.CreateDirectory(hedefKlasor);

            var dosyaAdi = $"{Guid.NewGuid():N}{uzanti}";
            var tamYol = Path.Combine(hedefKlasor, dosyaAdi);

            await using (var stream = new FileStream(tamYol, FileMode.Create))
            {
                await dosya.CopyToAsync(stream);
            }

            // Tarayicida acilabilir tam URL (gateway arkasinda da goreceli yol calisir).
            var url = $"{Request.Scheme}://{Request.Host}/images/{dosyaAdi}";
            return Ok(ServisSonuc<string>.Basarili(url, 201));
        }
    }
}
