using Microsoft.Extensions.DependencyInjection.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoPrism.Core;
using UmbracoPrism.Core.Services;

namespace ServiceDesignCompanion.Site.App;

[ComposeAfter(typeof(PrismComposer))]
public sealed class AppComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder) =>
        builder.Services.Replace(ServiceDescriptor.Scoped<IPrismMobileTabsProvider, SiteMobileTabs>());
}
