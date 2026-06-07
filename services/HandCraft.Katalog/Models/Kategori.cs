using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HandCraft.Katalog.Models
{
    // El-isi urun kategorisi (Seramik, Ahsap, Taki, Orgu vb.).
    public class Kategori
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("ad")]
        public string Ad { get; set; } = string.Empty;
    }
}
