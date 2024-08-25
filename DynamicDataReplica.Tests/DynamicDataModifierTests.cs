using DynamicDataReplica.Tests.Models;

namespace DynamicDataReplica.Tests;

[TestClass]
public class DynamicDataModifierTests
{
    [TestMethod]
    public void ShadowCloneWithModifierTest()
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

        var modifier = new SampeleModifier();
        dynamic shadowClone = DynamicDataReplica.ShadowCloneWithModifier(instance, modifier);

        Assert.AreEqual<string>("modifiedPropertyA", shadowClone.PropertyA);
        Assert.AreNotEqual<string>("modifiedArrayProp", shadowClone.PropertyB.ArrayProp[0]);
        Assert.AreNotEqual<string>("modifiedName", shadowClone.PropertyB.DictProp["key"].Name);
        Assert.AreNotEqual<string>("modifiedDescription", shadowClone.PropertyB.DictProp["key"].Description);
        Assert.AreNotEqual<int>(2, shadowClone.PropertyC.Id);
        Assert.AreNotEqual<string>("modifiedName", shadowClone.PropertyC.Name);
        Assert.AreNotEqual<string>("modifiedDescription", shadowClone.PropertyC.Description);
    }

    [TestMethod]
    public void DeepCloneWithModifierTest()
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

        var modifier = new SampeleModifier();
        dynamic deepClone = DynamicDataReplica.DeepCloneWithModifier(instance, modifier);

        Assert.AreEqual<string>("modifiedPropertyA", deepClone.PropertyA);
        Assert.AreEqual<string>("modifiedArrayProp", deepClone.PropertyB.ArrayProp[0]);
        Assert.AreEqual<string>("modifiedName", deepClone.PropertyB.DictProp["key"].Name);
        Assert.AreEqual<string>("modifiedDescription", deepClone.PropertyB.DictProp["key"].Description);
        Assert.AreEqual(100, deepClone.PropertyB.ListProp[0].DictProp["key"]);
        Assert.AreEqual(100, deepClone.PropertyB.ListProp[0].ListProp[0]);
        Assert.AreEqual<int>(2, deepClone.PropertyC.Id);
        Assert.AreEqual<string>("modifiedName", deepClone.PropertyC.Name);
        Assert.AreEqual<string>("modifiedDescription", deepClone.PropertyC.Description);
    }

    private class SampeleModifier : IValueModifier
    {
        private Dictionary<string, object> propMap = new Dictionary<string, object>()
        {
            { "PropertyA", "modifiedPropertyA" },
            { "PropertyB.ArrayProp[0]", "modifiedArrayProp" },
            { "PropertyB.DictProp[key].Name", "modifiedName" },
            { "PropertyB.DictProp[key].Description", "modifiedDescription" },
            { "PropertyB.ListProp[0].DictProp[key]", 100 },
            { "PropertyB.ListProp[0].ListProp[0]", 100 },
            { "PropertyC.Id", 2 },
            { "PropertyC.Name", "modifiedName" },
            { "PropertyC.Description", "modifiedDescription" },
            { "PropertyD.ListProp[0]", 1 }
        };

        public bool TryUpdateValue(string propertyPath, object _, out object? modifiedValue)
        {
            return propMap.TryGetValue(propertyPath, out modifiedValue);
        }
    }
}