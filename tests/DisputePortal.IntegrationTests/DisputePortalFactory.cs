using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace DisputePortal.IntegrationTests;

public class DisputePortalFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", _db.GetConnectionString());
        builder.UseSetting("Jwt:Issuer", "DisputePortal");
        builder.UseSetting("Jwt:Audience", "DisputePortal");
        builder.UseSetting("Jwt:Key", "test-only-secret-key-min-32-characters-long!!");
        builder.UseSetting("Jwt:ExpiryMinutes", "60");
    }

    public async Task InitializeAsync() => await _db.StartAsync(); 
    public new async Task DisposeAsync() => await _db.DisposeAsync();
}