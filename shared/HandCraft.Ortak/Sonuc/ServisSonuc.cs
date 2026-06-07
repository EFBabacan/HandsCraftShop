using System.Text.Json.Serialization;

namespace HandCraft.Ortak.Sonuc
{
    // Tum controller'larin dondurdugu ortak sonuc sarmalayicisi (veri yok).
    // Hocanin "ServiceResult" kalibinin Turkce karsiligi: ServisSonuc.
    public class ServisSonuc
    {
        public bool BasariliMi { get; set; }

        public int StatusCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ServisHataDto? Hatalar { get; set; }

        [JsonIgnore]
        public bool BasarisizMi => !BasariliMi;

        public static ServisSonuc Basarili(int statusCode = 200)
        {
            return new ServisSonuc { BasariliMi = true, StatusCode = statusCode };
        }

        public static ServisSonuc Hata(string mesaj, int statusCode = 400)
        {
            return new ServisSonuc
            {
                BasariliMi = false,
                StatusCode = statusCode,
                Hatalar = new ServisHataDto(mesaj)
            };
        }

        public static ServisSonuc Hata(IEnumerable<string> mesajlar, int statusCode = 400)
        {
            return new ServisSonuc
            {
                BasariliMi = false,
                StatusCode = statusCode,
                Hatalar = new ServisHataDto(mesajlar)
            };
        }
    }

    // Veri tasiyan genel sonuc.
    public class ServisSonuc<T> : ServisSonuc
    {
        public T? Data { get; set; }

        public static ServisSonuc<T> Basarili(T data, int statusCode = 200)
        {
            return new ServisSonuc<T> { BasariliMi = true, StatusCode = statusCode, Data = data };
        }

        public static new ServisSonuc<T> Hata(string mesaj, int statusCode = 400)
        {
            return new ServisSonuc<T>
            {
                BasariliMi = false,
                StatusCode = statusCode,
                Hatalar = new ServisHataDto(mesaj)
            };
        }

        public static new ServisSonuc<T> Hata(IEnumerable<string> mesajlar, int statusCode = 400)
        {
            return new ServisSonuc<T>
            {
                BasariliMi = false,
                StatusCode = statusCode,
                Hatalar = new ServisHataDto(mesajlar)
            };
        }
    }
}
