using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ServiceDesignCompanion.Site.Tests;

[CollectionDefinition(Name)]
public sealed class SharedBootedSite : ICollectionFixture<BootedSite>
{
    public const string Name = "Booted site";
}

/// <summary>The real site on a fresh SQLite database in a temporary folder, installed unattended.</summary>
public sealed class BootedSite : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "psd-tests-" + Guid.NewGuid().ToString("N"));

    public HttpClient Browser() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost"),
    });

    /// <summary>A page's HTML with entities decoded, so assertions read as the visitor sees the text.</summary>
    public async Task<string> PageTextAsync(string path)
    {
        using var browser = Browser();
        return await TextOf(browser, path);
    }

    /// <summary>The same, through a browser that keeps its cookies between requests.</summary>
    public static async Task<string> TextOf(HttpClient browser, string path) => System.Net.WebUtility.HtmlDecode(await browser.GetStringAsync(path));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var data = Path.Combine(_root, "umbraco", "Data");
        Directory.CreateDirectory(data);
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:umbracoDbDSN"] = $"Data Source={Path.Combine(data, "Umbraco.sqlite.db")};Foreign Keys=True;Pooling=True",
            ["ConnectionStrings:umbracoDbDSN_ProviderName"] = "Microsoft.Data.Sqlite",
            ["Umbraco:CMS:Unattended:InstallUnattended"] = "true",
            ["Umbraco:CMS:Unattended:UnattendedUserName"] = "Tests",
            ["Umbraco:CMS:Unattended:UnattendedUserEmail"] = "tests@example.test",
            ["Umbraco:CMS:Unattended:UnattendedUserPassword"] = "booted-site-tests-only",
        }));
    }

    public async Task InitializeAsync()
    {
        using var browser = Browser();
        var deadline = DateTime.UtcNow.AddMinutes(4);
        while (DateTime.UtcNow < deadline)
        {
            // The seeder publishes the method cards last, in order, so the last one means the whole site is there.
            var response = await browser.GetAsync("/methods/team-charter/");
            if (response.IsSuccessStatusCode) return;
            await Task.Delay(1000);
        }

        throw new TimeoutException("The site did not finish seeding within four minutes.");
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { }
    }
}
