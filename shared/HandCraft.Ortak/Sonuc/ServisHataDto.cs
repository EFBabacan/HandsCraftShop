namespace HandCraft.Ortak.Sonuc
{
    // Standart hata govdesi: tek mesaj veya alan-bazli dogrulama hatalari listesi.
    public class ServisHataDto
    {
        public List<string> Mesajlar { get; set; } = new();

        public ServisHataDto() { }

        public ServisHataDto(string mesaj)
        {
            Mesajlar.Add(mesaj);
        }

        public ServisHataDto(IEnumerable<string> mesajlar)
        {
            Mesajlar.AddRange(mesajlar);
        }
    }
}
