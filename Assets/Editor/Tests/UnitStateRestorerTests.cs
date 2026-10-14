using NUnit.Framework;
using System.Collections.Generic;

public class UnitStateRestorerTests
{
    private Dictionary<string, Unit> templates;

    [SetUp]
    public void SetUp()
    {
        templates = new Dictionary<string, Unit>
        {
            ["id-a"] = new Unit
            {
                Id = "id-a",
                UnitName = "Kusznicy",
                Faction = "A",
                Durability = 10,
                SoldierCount = 100,
                TemplateSoldierCount = 100
            }
        };
    }

    [Test]
    public void Restore_ValidSave_AppliesSavedState()
    {
        // arrange
        var save = new UnitSaveData
        {
            TemplateId = "id-a",
            UnitName = "Kusznicy",
            FactionID = "A",
            StartingSoldierCount = 90,
            CurrentSoldierCount = 50,
            IsBroken = true
        };

        // act
        Unit unit = UnitStateRestorer.Restore(save, templates, out _);

        // assert
        Assert.AreEqual("id-a", unit.TemplateId);
        Assert.AreEqual(90, unit.StartingSoldierCount);
        Assert.AreEqual(50, unit.SoldierCount);
        Assert.IsTrue(unit.IsBroken);
    }

    [Test]
    public void Restore_NegativeSoldierCount_ClampsToZero()
    {
        // arrange
        var save = new UnitSaveData
        {
            TemplateId = "id-a",
            UnitName = "Kusznicy",
            FactionID = "A",
            CurrentSoldierCount = -5
        };

        // act
        Unit unit = UnitStateRestorer.Restore(save, templates, out _);

        // assert
        Assert.AreEqual(0, unit.SoldierCount);
    }

    [Test]
    public void Restore_UnknownTemplate_ReturnsNull()
    {
        // arrange
        var save = new UnitSaveData { UnitName = "Nieznana", FactionID = "A" };

        // act
        Unit unit = UnitStateRestorer.Restore(save, templates, out TemplateMatch match);

        // assert
        Assert.IsNull(unit);
        Assert.AreEqual(TemplateMatch.None, match);
    }
}
