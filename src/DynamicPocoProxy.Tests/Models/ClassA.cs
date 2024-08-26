using System.Text.Json.Serialization;

namespace DynamicPocoProxy.Tests.Models
{
    internal class ClassA
    {
        public ClassA(string propertyA, ClassB propertyB, ClassC propertyC, ClassD propertyD)
        {
            PropertyA = propertyA;
            PropertyB = propertyB;
            PropertyC = propertyC;
            PropertyD = propertyD;
        }

        [JsonPropertyName("A")]
        public string PropertyA { get; }

        [JsonPropertyName("B")]
        public ClassB PropertyB { get; }

        [JsonPropertyName("C")]
        public ClassC PropertyC { get; }

        [JsonPropertyName("D")]
        public ClassD PropertyD { get; }
    }
}
