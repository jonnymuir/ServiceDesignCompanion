using ServiceDesignCompanion.Site.Content;

namespace ServiceDesignCompanion.Site.Tests;

[Collection(SharedBootedSite.Name)]
public sealed class MethodPagesTests(BootedSite site)
{
    [Fact]
    public async Task TheMethodsPage_ListsEveryMethodCard()
    {
        var html = await site.PageTextAsync("/methods/");

        html.Should().Contain($"Showing {MethodCards.All.Count} of {MethodCards.All.Count} methods");
        MethodCards.All.Should().AllSatisfy(card => html.Should().Contain($">{card.Title}</a>"));
    }

    [Fact]
    public async Task FilteringByPhaseAndPoint_ShowsOnlyTheMethodsThatMatchBoth()
    {
        var expected = MethodCards.All.Where(card => card.Phases.Contains(Phase.Alpha) && card.Points.Contains(4)).ToList();

        var html = await site.PageTextAsync("/methods/?phase=alpha&point=4");

        html.Should().Contain($"Showing {expected.Count} of {MethodCards.All.Count} methods");
        foreach (var card in MethodCards.All)
        {
            if (expected.Contains(card)) html.Should().Contain($">{card.Title}</a>");
            else html.Should().NotContain($">{card.Title}</a>");
        }
    }

    [Fact]
    public async Task AMethodPage_ShowsWhenAndHowToUseIt_AndLinksToThePointsItHelps()
    {
        var card = MethodCards.All.Single(c => c.Title == "Usability testing");

        var html = await site.PageTextAsync("/methods/usability-testing/");

        html.Should().Contain(card.Summary).And.Contain(card.WhenToUse).And.Contain(card.Steps[^1]);
        html.Should().Contain(">Alpha</strong>").And.Contain(">Live</strong>").And.NotContain(">Discovery</strong>");
        html.Should().Contain("Point 4: Make the service simple to use</a>").And.Contain("Point 5: Make sure everyone can use the service</a>");
    }

    [Fact]
    public async Task APointPage_ListsTheMethodsThatHelpWithIt()
    {
        var html = await site.PageTextAsync("/the-service-standard/create-a-secure-service-which-protects-users-privacy/");

        html.Should().Contain("Methods that help");
        MethodCards.All.Where(card => card.Points.Contains(9)).Should().AllSatisfy(card => html.Should().Contain($">{card.Title}</a>"));
    }
}
