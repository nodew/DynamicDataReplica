using System.Text.Json.Serialization;

namespace DynamicPocoProxy.Tests.Models
{
    internal class ClassD
    {
        public ClassD(Dictionary<string, int> dictProp, List<int> listProp)
        {
            DictProp = dictProp;
            ListProp = listProp;
        }

        public Dictionary<string, int> DictProp { get; set; }

        [JsonIgnore]
        public List<int> ListProp { get; set; }
    }
}
