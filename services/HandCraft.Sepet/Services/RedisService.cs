using StackExchange.Redis;

namespace HandCraft.Sepet.Services
{
    // Singleton Redis baglantisi (Bolum 4 madde 9).
    // ConnectionMultiplexer.Connect($"{host}:{port}") -> GetDatabase(db).
    public class RedisService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly int _db;
        private ConnectionMultiplexer _connection = default!;

        public RedisService(IConfiguration configuration)
        {
            _host = configuration["Redis:Host"] ?? "localhost";
            _port = int.TryParse(configuration["Redis:Port"], out var p) ? p : 6379;
            _db = int.TryParse(configuration["Redis:Db"], out var d) ? d : 0;
        }

        public void Connect()
        {
            _connection = ConnectionMultiplexer.Connect($"{_host}:{_port}");
        }

        public IDatabase GetDatabase() => _connection.GetDatabase(_db);

        public System.Net.EndPoint[] GetEndPoints() => _connection.GetEndPoints();

        public IServer GetServer() => _connection.GetServer(_connection.GetEndPoints().First());
    }
}
