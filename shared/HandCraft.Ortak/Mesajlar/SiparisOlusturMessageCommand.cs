namespace HandCraft.Ortak.Mesajlar
{
    // RabbitMQ kontrati (Bolum 4 madde 11):
    // Odeme servisi publish eder -> queue:siparis-olustur-service
    // Siparis.Application'daki SiparisOlusturConsumer tuketir.
    // MassTransit referansi GEREKMEZ; saf POCO mesaj kontrati.
    public class SiparisOlusturMessageCommand
    {
        public SiparisDto Siparis { get; set; } = new();
    }
}
