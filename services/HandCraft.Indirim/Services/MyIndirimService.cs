using Dapper;
using HandCraft.Indirim.Dtos;
using HandCraft.Ortak.Sonuc;
using Npgsql;

namespace HandCraft.Indirim.Services
{
    // PostgreSQL + Dapper + ham SQL (Bolum 4 madde 10). EF Core KULLANILMAZ.
    public class MyIndirimService : IMyIndirimService
    {
        private readonly string _connStr;

        public MyIndirimService(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("PostgreSql")
                       ?? throw new InvalidOperationException("PostgreSql baglanti dizesi bulunamadi.");
        }

        private NpgsqlConnection Baglanti() => new NpgsqlConnection(_connStr);

        // Acilista 'indirim' tablosunu yoksa olusturur (Bolum 7/Faz 3).
        public async Task TabloyuHazirlaAsync()
        {
            const string sql = @"
                create table if not exists indirim (
                    id        serial primary key,
                    user_id   varchar(100) not null,
                    oran      numeric(5,2) not null,
                    kod       varchar(50)  not null unique,
                    is_active boolean      not null default true
                );";
            await using var conn = Baglanti();
            await conn.ExecuteAsync(sql);
        }

        public async Task<ServisSonuc<List<IndirimDto>>> HepsiniGetirAsync()
        {
            const string sql = "select id, user_id as UserId, oran, kod, is_active as IsActive from indirim order by id";
            await using var conn = Baglanti();
            var liste = (await conn.QueryAsync<IndirimDto>(sql)).ToList();
            return ServisSonuc<List<IndirimDto>>.Basarili(liste);
        }

        public async Task<ServisSonuc<IndirimDto>> IdileGetirAsync(int id)
        {
            const string sql = "select id, user_id as UserId, oran, kod, is_active as IsActive from indirim where id = @id";
            await using var conn = Baglanti();
            var kayit = await conn.QueryFirstOrDefaultAsync<IndirimDto>(sql, new { id });
            if (kayit is null)
                return ServisSonuc<IndirimDto>.Hata("Indirim bulunamadi.", 404);

            return ServisSonuc<IndirimDto>.Basarili(kayit);
        }

        public async Task<ServisSonuc<IndirimDto>> KodIleGetirAsync(string kod)
        {
            const string sql = "select id, user_id as UserId, oran, kod, is_active as IsActive from indirim where kod = @kod and is_active = true";
            await using var conn = Baglanti();
            var kayit = await conn.QueryFirstOrDefaultAsync<IndirimDto>(sql, new { kod });
            if (kayit is null)
                return ServisSonuc<IndirimDto>.Hata("Aktif indirim kodu bulunamadi.", 404);

            return ServisSonuc<IndirimDto>.Basarili(kayit);
        }

        public async Task<ServisSonuc<IndirimDto>> EkleAsync(IndirimCreateDto dto)
        {
            const string sql = @"
                insert into indirim (user_id, oran, kod, is_active)
                values (@UserId, @Oran, @Kod, @IsActive)
                returning id, user_id as UserId, oran, kod, is_active as IsActive;";
            await using var conn = Baglanti();
            try
            {
                var kayit = await conn.QueryFirstAsync<IndirimDto>(sql, dto);
                return ServisSonuc<IndirimDto>.Basarili(kayit, 201);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // unique_violation
            {
                return ServisSonuc<IndirimDto>.Hata("Bu indirim kodu zaten mevcut.", 409);
            }
        }

        public async Task<ServisSonuc> GuncelleAsync(IndirimUpdateDto dto)
        {
            const string sql = @"
                update indirim
                set user_id = @UserId, oran = @Oran, kod = @Kod, is_active = @IsActive
                where id = @Id;";
            await using var conn = Baglanti();
            var etkilenen = await conn.ExecuteAsync(sql, dto);
            if (etkilenen == 0)
                return ServisSonuc.Hata("Guncellenecek indirim bulunamadi.", 404);

            return ServisSonuc.Basarili();
        }

        public async Task<ServisSonuc> SilAsync(int id)
        {
            const string sql = "delete from indirim where id = @id";
            await using var conn = Baglanti();
            var etkilenen = await conn.ExecuteAsync(sql, new { id });
            if (etkilenen == 0)
                return ServisSonuc.Hata("Silinecek indirim bulunamadi.", 404);

            return ServisSonuc.Basarili();
        }
    }
}
