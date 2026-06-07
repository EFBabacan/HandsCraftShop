namespace HandCraft.Katalog.Settings
{
    // appsettings.json -> "MongoDbSettings" bolumunden IOptions ile baglanir (Bolum 4 madde 8).
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }

    // Koleksiyon (tablo) isimleri sabit olarak burada (Bolum 4 madde 8).
    public static class MongoDbTables
    {
        public const string Urun = "urun";
        public const string Kategori = "kategori";
        public const string Yorum = "yorum";
    }
}
