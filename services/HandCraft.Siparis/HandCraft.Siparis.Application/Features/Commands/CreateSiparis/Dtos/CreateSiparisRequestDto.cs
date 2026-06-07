namespace HandCraft.Siparis.Application.Features.Commands.CreateSiparis.Dtos
{
    // Siparis olusturma istegi (Api POST body veya RabbitMQ mesajindan doldurulur).
    public class CreateSiparisRequestDto
    {
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;
        public List<CreateSiparisUrunDto> Urunler { get; set; } = new();
    }

    public class CreateSiparisUrunDto
    {
        public string UrunId { get; set; } = string.Empty;
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }
}
