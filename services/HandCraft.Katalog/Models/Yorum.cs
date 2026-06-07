using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HandCraft.Katalog.Models
{
    // Urune ait yorum + puan (1-5). MongoDB 'yorum' koleksiyonu.
    public class Yorum
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("urunId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UrunId { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("kullaniciAdi")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [BsonElement("puan")]
        public int Puan { get; set; }   // 1-5

        [BsonElement("metin")]
        public string Metin { get; set; } = string.Empty;

        [BsonElement("tarih")]
        public DateTime Tarih { get; set; }
    }
}
