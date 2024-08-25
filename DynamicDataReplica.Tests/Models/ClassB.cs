using System.Text.Json.Serialization;

namespace DynamicDataReplica.Tests.Models
{
    internal class ClassB
    {
        public ClassB(string[] arrayProp, Dictionary<string, ClassC> dictProp, List<ClassD> listProp)
        {
            ArrayProp = arrayProp;
            DictProp = dictProp;
            ListProp = listProp;
        }

        [JsonPropertyName("Array")]
        public string[] ArrayProp { get; }

        [JsonPropertyName("Dict")]
        public Dictionary<string, ClassC> DictProp { get; }

        [JsonPropertyName("List")]
        public List<ClassD> ListProp { get; }
    }
}
