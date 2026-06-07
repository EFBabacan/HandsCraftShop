namespace HandCraft.Indirim.Dtos
{
    // PostgreSQL 'indirim' tablosunun satir karsiligi (Dapper map'ler).
    public class IndirimDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Oran { get; set; }      // yuzde indirim orani ( or. 10 = %10)
        public string Kod { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // Indirim olusturma istegi.
    public class IndirimCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Oran { get; set; }
        public string Kod { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    // Indirim guncelleme istegi.
    public class IndirimUpdateDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Oran { get; set; }
        public string Kod { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
