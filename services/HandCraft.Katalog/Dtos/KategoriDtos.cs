namespace HandCraft.Katalog.Dtos
{
    // Disari donen kategori gorunumu.
    public class KategoriViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
    }

    // Kategori olusturma istegi.
    public class KategoriCreateDto
    {
        public string Ad { get; set; } = string.Empty;
    }

    // Kategori guncelleme istegi.
    public class KategoriUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
    }
}
