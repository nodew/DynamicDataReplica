namespace DynamicPocoProxy.Tests;

[TestClass]
public class DynamicDataReplicaWithNewOverrideTest
{
    [TestMethod]
    public void TestDeepCloneClassInstanceWithNewOverride()
    {
        var bar = new Bar();
        dynamic replica = DynamicDataReplica.DeepClone(bar);

        Assert.AreEqual(bar.Uri, replica.Uri);
    }

    private class Foo
    {
        public Uri Uri => new Uri("http://www.example.com");
    }

    private class Bar : Foo
    {
        public new string Uri => "https://bar.example.com";
    }
}
