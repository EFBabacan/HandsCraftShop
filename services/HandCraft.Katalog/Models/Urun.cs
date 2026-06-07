using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HandCraft.Katalog.Models
{
    // El-isi urun.
    public class Urun
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("ad")]
        public string Ad { get; set; } = string.Empty;

        [BsonElement("aciklama")]
        public string Aciklama { get; set; } = string.Empty;

        [BsonElement("fiyat")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Fiyat { get; set; }

        [BsonElement("kategoriId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string KategoriId { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("stok")]
        public int Stok { get; set; }

        [BsonElement("eklenmeTarihi")]
        public DateTime EklenmeTarihi { get; set; }
    }
}
