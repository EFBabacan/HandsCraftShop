namespace HandCraft.Siparis.Domain.Entities
{
    // Teslimat adresi. Siparis icine owned tip olarak gomulur.
    public class Address
    {
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string AcikAdres { get; set; } = string.Empty;
    }
}
