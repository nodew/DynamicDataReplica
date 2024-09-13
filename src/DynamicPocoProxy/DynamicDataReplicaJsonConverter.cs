using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicPocoProxy;

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
                var propertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>(false)?.Name ?? property.Name;

                var jsonIgnoreAttribute = property.GetCustomAttribute<JsonIgnoreAttribute>();

                if (value.TryGetMemberByName(property.Name, out var propValue))
                {
                    if (jsonIgnoreAttribute is not null)
                    {
                        if (jsonIgnoreAttribute.Condition == JsonIgnoreCondition.Always)
                        {
                            continue;
                        }

                        if (jsonIgnoreAttribute.Condition == JsonIgnoreCondition.WhenWritingDefault && propValue is null)
                        {
                            continue;
                        }

                        if (jsonIgnoreAttribute.Condition == JsonIgnoreCondition.WhenWritingNull && propValue is null)
                        {
                            continue;
                        }
                    }

                    // Trick to align with System.Text.Json serilization bug https://github.com/dotnet/runtime/issues/92780

                    var getMethod = property.GetGetMethod(false);
                    if (getMethod?.GetBaseDefinition() != getMethod)
                    {
                        if (jsonPropertyName != propertyName)
                        {
                            writer.WritePropertyName(propertyName);
                            JsonSerializer.Serialize(writer, propValue, options);
                        }
                    }


                    writer.WritePropertyName(jsonPropertyName);
                    JsonSerializer.Serialize(writer, propValue, options);
                }
            }
        }

        writer.WriteEndObject();
    }
}
