using NUnit.Framework;
using System.Collections.Generic;

public class TemplateFinderTests
{
    private Dictionary<string, Unit> templates;

    [SetUp]
    public void SetUp()
    {
        // Two factions with a unit of the same name: the reported bug.
        // Faction A is added first, so a name-only search would return it.
        templates = new Dictionary<string, Unit>
        {
            ["id-a"] = new Unit { Id = "id-a", UnitName = "Kolizja", Faction = "A" },
            ["id-b"] = new Unit { Id = "id-b", UnitName = "Kolizja", Faction = "B"}
        };
    }

    [Test]
    public void Find_WithTemplateId_ReturnsExactTemplate()
    {
        // arrange
        Unit result = TemplateFinder.Find(templates, "id-b", "Kolizja", "B", out TemplateMatch match);

        // act

        // assert
        Assert.AreEqual("id-b", result.Id);
        Assert.AreEqual(TemplateMatch.ById, match);
    }

    [Test]
    public void Find_OldSaveWithoutId_ReturnsTemplateFromSameFaction()
    {
        // arrange
        Unit result = TemplateFinder.Find(templates, null, "Kolizja", "B", out TemplateMatch match);

        // act

        // assert
        Assert.AreEqual("id-b", result.Id);
        Assert.AreEqual(TemplateMatch.ByNameAndFaction, match);
    }

    [Test]
    public void Find_FactionNotFound_FallsBackToNameOnly()
    {
        // arrange
        Unit result = TemplateFinder.Find(templates, null, "Kolizja", "C", out TemplateMatch match);

        // act

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TemplateMatch.ByNameOnly, match);
    }

    [Test]
    public void Find_UnknownName_ReturnsNull()
    {
        // arrange
        Unit result = TemplateFinder.Find(templates, null, "Nieznana", "A", out TemplateMatch match);

        // act

        // assert
        Assert.IsNull(result);
        Assert.AreEqual(TemplateMatch.None, match);
    }
}
