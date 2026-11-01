using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace ServiceDesignCompanion.Site.Content;

public sealed class SiteContentComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<SiteContentTypes>();
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, SiteContentSeeder>();
    }
}
