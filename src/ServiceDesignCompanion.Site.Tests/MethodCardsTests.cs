using ServiceDesignCompanion.Site.Content;

namespace ServiceDesignCompanion.Site.Tests;

public sealed class MethodCardsTests
{
    [Fact]
    public void EveryCard_HasAUniqueTitle() =>
        MethodCards.All.Select(card => card.Title).Should().OnlyHaveUniqueItems();

    [Fact]
    public void EveryCard_SuitsAtLeastOnePhase_AndHelpsWithRealPoints()
    {
        MethodCards.All.Should().AllSatisfy(card =>
        {
            card.Phases.Should().NotBeEmpty(because: card.Title);
            card.Points.Should().NotBeEmpty(because: card.Title).And.OnlyContain(n => n >= 1 && n <= 14, because: card.Title);
        });
    }

    [Fact]
    public void EveryPoint_HasAtLeastOneMethodThatHelps_ExceptTheTechnologyPoints()
    {
        int[] technologyPoints = [11, 12];
        var covered = MethodCards.All.SelectMany(card => card.Points).ToHashSet();

        ServiceStandard.Points.Select(p => p.Number).Except(technologyPoints).Should().BeSubsetOf(covered);
    }

    [Fact]
    public void NoCard_UsesAnEmDash() =>
        MethodCards.All.Should().AllSatisfy(card =>
            string.Join(' ', [card.Title, card.Summary, card.WhenToUse, .. card.Steps]).Should().NotContain("—", because: card.Title));
}
