using DynamicPocoProxy.Tests.Models;

namespace DynamicPocoProxy.Tests;

[TestClass]
public class DynamicDataReplicaTests
{
    [TestMethod]
    public void ShallowCloneTests()
    {
        var instance = new ClassA(
            "propertyA",
            new ClassB(
                new[] { "arrayProp" },
                new Dictionary<string, ClassC> { { "key", new ClassC(1, "name", "description") } },
                new List<ClassD> {
                    new ClassD(
                        new Dictionary<string, int> { { "key", 1 } },
                        new List<int> { 1 }) }),
            new ClassC(1, "name", "description"),
            new ClassD(
                new Dictionary<string, int> { { "key", 1 } },
                new List<int> { 1 }));

        dynamic shadowClone = DynamicDataReplica.ShallowClone(instance);

        Assert.AreEqual(instance.PropertyA, shadowClone.PropertyA);
        Assert.AreEqual(instance.PropertyB, shadowClone.PropertyB);
        Assert.AreEqual(instance.PropertyC, shadowClone.PropertyC);
        Assert.AreEqual(instance.PropertyD, shadowClone.PropertyD);
    }

    [TestMethod]
    public void ShallowCloneTests_WithNulls()
    {
        var instance = new ClassA(
            "propertyA",
            new ClassB(
                new[] { "arrayProp" },
                new Dictionary<string, ClassC> { { "key", new ClassC(1, "name", "description") } },
                new List<ClassD> {
                    new ClassD(
                        new Dictionary<string, int> { { "key", 1 } },
                        new List<int> { 1 }) }),
            null,
            null);

        dynamic shadowClone = DynamicDataReplica.ShallowClone(instance);

        Assert.AreEqual(instance.PropertyA, shadowClone.PropertyA);
        Assert.AreEqual(instance.PropertyB, shadowClone.PropertyB);
        Assert.IsNull(shadowClone.PropertyC);
        Assert.IsNull(shadowClone.PropertyD);
    }

    [TestMethod]
    public void DeepCloneTests()
    {
        var instance = new ClassA(
            "propertyA",
            new ClassB(
                new[] { "arrayProp" },
                new Dictionary<string, ClassC> { { "key", new ClassC(1, "name", "description") } },
                new List<ClassD> {
                    new ClassD(
                        new Dictionary<string, int> { { "key", 1 } },
                        new List<int> { 1 }) }),
            new ClassC(1, "name", "description"),
            new ClassD(
                new Dictionary<string, int> { { "key", 1 } },
                new List<int> { 1 }));

        dynamic deepClone = DynamicDataReplica.DeepClone(instance);

        Assert.AreEqual(instance.PropertyA, deepClone.PropertyA);

        Assert.AreNotEqual<object>(instance.PropertyB, deepClone.PropertyB);
        Assert.AreNotEqual<object>(instance.PropertyC, deepClone.PropertyC);

        Assert.AreEqual(instance.PropertyB.DictProp["key"].Id, deepClone.PropertyB.DictProp["key"].Id);
        Assert.AreEqual(instance.PropertyB.DictProp["key"].Name, deepClone.PropertyB.DictProp["key"].Name);
        Assert.AreEqual(instance.PropertyB.DictProp["key"].Description, deepClone.PropertyB.DictProp["key"].Description);
        Assert.AreEqual(instance.PropertyB.ListProp[0].DictProp["key"], deepClone.PropertyB.ListProp[0].DictProp["key"]);
        Assert.AreEqual(instance.PropertyB.ListProp[0].ListProp[0], deepClone.PropertyB.ListProp[0].ListProp[0]);
        Assert.AreEqual(instance.PropertyC.Id, deepClone.PropertyC.Id);
        Assert.AreEqual(instance.PropertyC.Name, deepClone.PropertyC.Name);
        Assert.AreEqual(instance.PropertyC.Description, deepClone.PropertyC.Description);
        Assert.AreEqual(instance.PropertyD.DictProp["key"], deepClone.PropertyD.DictProp["key"]);
        Assert.AreEqual(instance.PropertyD.ListProp[0], deepClone.PropertyD.ListProp[0]);
    }
}