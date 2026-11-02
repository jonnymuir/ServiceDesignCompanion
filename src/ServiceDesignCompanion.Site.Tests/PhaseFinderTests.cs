using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ServiceDesignCompanion.Site.PhaseFinder;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Wayfinder.Engine.Abstractions;
using Wayfinder.Engine.Services;
using Wayfinder.Models.ServiceDesign;

namespace ServiceDesignCompanion.Site.Tests;

[Collection(SharedBootedSite.Name)]
public sealed partial class PhaseFinderTests(BootedSite site)
{
    private const string PagePath = "/phase-finder/";
    private static readonly string[] HiddenFields = ["__RequestVerificationToken", "InstanceId", "StateVersion", "BlueprintKey", "ReturnUrl", "Nonce"];

    public static TheoryData<string, string, string, string, string, string> Answers => new()
    {
        { "Not yet", "Not yet", "Not yet", "No, not yet", "Not yet", "You are probably in discovery" },
        { "Some, from early research", "Yes", "Yes, with prototypes", "No, not yet", "Not yet", "You are probably in alpha" },
        { "Yes, from research we keep doing", "Yes", "Yes, with a working service", "Yes, a limited group can use it", "Not yet", "You are probably in beta" },
        { "Yes, from research we keep doing", "Yes", "Yes, with a working service", "Yes, anyone can use it", "Yes", "You are probably live" },
    };

    [Theory]
    [MemberData(nameof(Answers))]
    public async Task AnsweringTheQuestions_ShowsTheLikelyPhase(string evidence, string problemAgreed, string tested, string realUse, string replaced, string expected)
    {
        using var browser = site.Browser();

        var result = await FindPhaseAsync(browser, evidence, problemAgreed, tested, realUse, replaced);

        result.Should().Contain(expected).And.Contain("not an assessment");
        var phase = expected.Split(' ')[^1];
        var shownPhases = ShownWhen().Matches(result).Select(m => m.Groups[1].Value).Distinct();
        shownPhases.Should().ContainSingle().Which.Should().Be($"is{char.ToUpperInvariant(phase[0])}{phase[1..]}", because: "only the likely phase is shown");
    }

    [Fact]
    public async Task OneVisitorsAnswers_AreNeverShownToAnother()
    {
        using var first = site.Browser();
        using var second = site.Browser();
        await FindPhaseAsync(first, "Not yet", "Not yet", "Not yet", "No, not yet", "Not yet");

        var page = await BootedSite.TextOf(second, PagePath);

        page.Should().Contain("Find my likely phase").And.NotContain("You are probably");
    }

    [Fact]
    public void TheSite_KeepsNoAuditTrailOfWhoUsedIt() =>
        site.Services.GetRequiredService<IAuditLogStore>().Should().BeOfType<NoAuditTrail>();

    [Fact]
    public void TheBlueprint_IsValid()
    {
        var blueprint = ServiceBlueprintYaml.Deserialize(File.ReadAllText(BlueprintPath()));

        var outcome = site.Services.GetRequiredService<ServiceBlueprintAuthoringService>().Validate(blueprint);

        outcome.IsValid.Should().BeTrue(because: string.Join("; ", outcome.Diagnostics.Select(d => $"{d.Code}: {d.Message}")));
    }

    [Fact]
    public async Task EveryLinkInTheBlueprint_GoesToAPageOnTheSite()
    {
        var links = LinkPattern().Matches(File.ReadAllText(BlueprintPath())).Select(m => m.Groups[1].Value).Distinct().ToList();
        links.Should().NotBeEmpty();

        using var browser = site.Browser();
        foreach (var link in links)
        {
            (await browser.GetAsync(link)).StatusCode.Should().Be(HttpStatusCode.OK, because: link);
        }
    }

    [Fact]
    public async Task ANextBoot_LeavesAnUnchangedBlueprintAlone()
    {
        var store = site.Services.GetRequiredService<IServiceBlueprintSourceStore>();
        var before = (await store.LoadAsync(PhaseFinderComposer.BlueprintKey))!.Version;

        using (var scope = site.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IEventAggregator>().PublishAsync(new UmbracoApplicationStartedNotification(isRestarting: false));
        }

        (await store.LoadAsync(PhaseFinderComposer.BlueprintKey))!.Version.Should().Be(before);
    }

    private static async Task<string> FindPhaseAsync(HttpClient browser, params string[] answers)
    {
        var form = await BootedSite.TextOf(browser, PagePath);
        using var content = new MultipartFormDataContent();
        foreach (var name in HiddenFields)
        {
            content.Add(new StringContent(Regex.Match(form, $"name=\"{name}\" value=\"([^\"]*)\"").Groups[1].Value), name);
        }

        string[] fields = ["evidence", "problemAgreed", "tested", "realUse", "replaced"];
        foreach (var (field, answer) in fields.Zip(answers))
        {
            content.Add(new StringContent(answer), $"field:{field}");
        }

        content.Add(new StringContent("find"), "Action");
        var response = await browser.PostAsync("/umbraco/wayfinder-stage/advance", content);
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        return await BootedSite.TextOf(browser, PagePath);
    }

    private string BlueprintPath() => Path.Combine(
        site.Services.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "service-blueprints", $"{PhaseFinderComposer.BlueprintKey}.yaml");

    [GeneratedRegex("data-wayfinder-show-when=\"([^\"]+)\">")]
    private static partial Regex ShownWhen();

    [GeneratedRegex("href=\"(/[^\"]*)\"")]
    private static partial Regex LinkPattern();
}
