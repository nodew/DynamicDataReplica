using DynamicPocoProxy.Tests.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicPocoProxy.Tests;

[TestClass]
public class DynamicDataReplicaSerializationTests
{
    [TestMethod]
    public void SerializeDynamicDataReplicaToJson()
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

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        options.Converters.Add(new DynamicDataReplicaJsonConverter());

        var replica = DynamicDataReplica.DeepClone(instance);

        var originalJson = JsonSerializer.Serialize(instance, options);
        var replicaJson = JsonSerializer.Serialize(replica, options);

        Assert.AreEqual(originalJson, replicaJson);
    }

    [TestMethod]
    public void SerializeDynamicDataReplicaToJson_WithJsonIgnore()
    {
        var foo = new Foo();
        var replica = DynamicDataReplica.DeepClone(foo);

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        options.Converters.Add(new DynamicDataReplicaJsonConverter());

        var json = JsonSerializer.Serialize(replica, options);

        var rootJsonElement = JsonDocument.Parse(json).RootElement;

        Assert.IsFalse(rootJsonElement.TryGetProperty("Count", out _));
    }

    [TestMethod]
    public void SerializeDynamicDataReplicaToJson_WithOverride()
    {
        var bar = new Bar();
        var replica = DynamicDataReplica.DeepClone(bar);

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        options.Converters.Add(new DynamicDataReplicaJsonConverter());

        var json = JsonSerializer.Serialize(replica, options);

        var rootJsonElement = JsonDocument.Parse(json).RootElement;

        Assert.IsTrue(rootJsonElement.TryGetProperty("Uri", out var value1));

        Assert.AreEqual("https://bar.example.com", value1.GetString());

        Assert.IsTrue(rootJsonElement.TryGetProperty("xUri", out var value2));

        Assert.AreEqual("https://bar.example.com", value2.GetString());
    }

    private class Foo
    {
        [JsonIgnore]
        public int Count => 1;

        [JsonPropertyName("xUri")]
        public virtual Uri Uri => new Uri("http://foo.example.com");
    }

    private class Bar : Foo
    {
        public override Uri Uri => new Uri("https://bar.example.com");
    }
}
