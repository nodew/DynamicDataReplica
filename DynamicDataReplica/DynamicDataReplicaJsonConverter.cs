using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDataReplica
{
    /// <summary>
    /// A custom JSON converter for <see cref="DynamicDataReplica"/> objects.
    /// </summary>
    public class DynamicDataReplicaJsonConverter : JsonConverter<DynamicDataReplica>
    {
        public override DynamicDataReplica? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DynamicDataReplica value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is not null)
            {
                var properties = value.GetTargetProperties();

                foreach (var property in properties)
                {
                    var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
                    var isIgnored = property.GetCustomAttribute<JsonIgnoreAttribute>() is not null;

                    if (isIgnored)
                    {
                        continue;
                    }

                    if (value.TryGetMemberByName(property.Name, out var propValue))
                    {
                        writer.WritePropertyName(jsonPropertyName);
                        JsonSerializer.Serialize(writer, propValue, options);
                    }
                }
            }

            writer.WriteEndObject();
        }
    }
}
