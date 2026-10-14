using NUnit.Framework;

public class UnitCloneTest
{
    [Test]
    public void CloneUnit_FromClone_KeepsOriginalTemplateId()
    {
        // arrange
        var template = new Unit { Id = "template-1", UnitName = "Kusznicy" };
        Unit firstClone = template.CloneUnit();

        // act
        Unit secondClone = firstClone.CloneUnit();

        // assert
        Assert.AreEqual("template-1", secondClone.TemplateId);
    }
}
