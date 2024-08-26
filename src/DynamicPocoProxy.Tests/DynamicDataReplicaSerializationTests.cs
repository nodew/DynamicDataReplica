using DynamicPocoProxy.Tests.Models;
using System.Text.Json;

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
}
