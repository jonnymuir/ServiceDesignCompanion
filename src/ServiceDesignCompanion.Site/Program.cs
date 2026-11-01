
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Umbraco writes a generated imaging key into appsettings.json when none is configured, which
// would put a secret in source control. Supply one for this process instead; production sets its own.
if (string.IsNullOrEmpty(builder.Configuration["Umbraco:CMS:Imaging:HMACSecretKey"]))
{
    builder.Configuration["Umbraco:CMS:Imaging:HMACSecretKey"] = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
}

// In production the site listens on loopback behind Cloudflare Tunnel, which terminates HTTPS.
// Loopback is a known proxy by default, so only the tunnel's forwarded headers are trusted.
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
        | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

app.UseForwardedHeaders();

await app.BootUmbracoAsync();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
