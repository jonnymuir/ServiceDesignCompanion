using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace ServiceDesignCompanion.Site.Security;

public sealed class SecurityHeadersComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder) =>
        builder.Services.Configure<UmbracoPipelineOptions>(options =>
            options.AddFilter(new UmbracoPipelineFilter("SecurityHeaders", app => app.UseMiddleware<SecurityHeadersMiddleware>())));
}

/// <summary>
/// Security headers for every response. Public pages also get a self-only Content-Security-Policy:
/// every script, style and font is served from this site, nothing is inline, and no page is meant
/// to be framed. The backoffice keeps Umbraco's own behaviour, since it relies on inline code.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    private const string ContentSecurityPolicy =
        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; " +
        "connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'";

    private static readonly PathString Backoffice = "/umbraco";

    // Under /umbraco only because Prism reserves that prefix; it is a public stylesheet, not backoffice UI.
    private static readonly PathString BrandingStylesheet = "/umbraco/prism/branding.css";

    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            Apply(context);
            return Task.CompletedTask;
        });
        return next(context);
    }

    private static void Apply(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers.XContentTypeOptions = "nosniff";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=()";
        if (context.Request.IsHttps)
        {
            headers.StrictTransportSecurity = "max-age=31536000";
        }

        if (IsBackoffice(context.Request.Path)) return;

        headers.ContentSecurityPolicy = ContentSecurityPolicy;
        headers.XFrameOptions = "DENY";
    }

    private static bool IsBackoffice(PathString path) =>
        path.StartsWithSegments(Backoffice, StringComparison.OrdinalIgnoreCase)
        && !path.Equals(BrandingStylesheet, StringComparison.OrdinalIgnoreCase);
}
