using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HandCraft.Siparis.Persistence.Context
{
    // Design-time (dotnet ef) migration uretimi icin. Calisma zamaninda kullanilmaz.
    public class SiparisDbContextFactory : IDesignTimeDbContextFactory<SiparisDbContext>
    {
        public SiparisDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<SiparisDbContext>()
                .UseSqlServer(
                    "Server=localhost,1433;Database=handcraft_siparis;User Id=sa;Password=HandCraft!2026;TrustServerCertificate=True;",
                    sql => sql.MigrationsAssembly(typeof(SiparisDbContext).Assembly.FullName))
                .Options;

            return new SiparisDbContext(options);
        }
    }
}
