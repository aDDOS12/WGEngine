using NUnit.Framework;
using UnityEngine;

public class UnitTokenTest
{
    private GameObject tokenObject;
    private UnitToken token;

    [SetUp]
    public void SetUp()
    {
        tokenObject = new GameObject("TestToken");
        token = tokenObject.AddComponent<UnitToken>();
    }

    [TearDown]
    public void TearDown()
    {
        // Destroy is deferred to the end of a frame and is not allowed in Edit Mode
        Object.DestroyImmediate(tokenObject);
    }

    [Test]
    public void RotateFacing_90Degrees_TurnsUpToLeft()
    {
        // arrange
        token.SetFacingDirection(Vector2.up);
        
        // act
        token.RotateFacing(90f);

        // assert
        // Floats need a tolerance: cos(90°) is not exactly 0 in float math
        Assert.AreEqual(-1f, token.FacingDirection.x, 0.0001f);
        Assert.AreEqual(0f, token.FacingDirection.y, 0.0001f);
    }

    [Test]
    public void RotateFacing_KeepsTransformInSync()
    {
        // arrange

        // act
        token.RotateFacing(45f);

        // assert
        Assert.AreEqual(token.FacingDirection.x, token.transform.up.x, 0.0001f);
        Assert.AreEqual(token.FacingDirection.y, token.transform.up.y, 0.0001f);
    }
}
