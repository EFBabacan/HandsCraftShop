namespace HandCraft.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---- YARP Reverse Proxy (appsettings "ReverseProxy") (Bolum 12) ----
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            // ---- CORS: Web (5000) ve Admin (5001) icin acik ----
            const string CorsPolicy = "HandCraftCors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, policy =>
                    policy.WithOrigins("http://localhost:5000", "http://localhost:5001")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            var app = builder.Build();

            app.UseCors(CorsPolicy);

            // Bearer token YARP tarafindan otomatik pass-through edilir (header'lar aynen iletilir).
            app.MapReverseProxy();

            app.Run();
        }
    }
}
