using System.Text.Json;
using System.Text.Json.Serialization;
using PluginAPI.Models.Permissions;

namespace ModularPluginAPI.Components.JsonConverters;

public class PermissionDictionaryConverter<T> : JsonConverter<Dictionary<string, T>> where T : PermissionBase
{
    public override Dictionary<string, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var permissions = JsonSerializer.Deserialize<List<T>>(ref reader, options);
        var dictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        
        if (permissions != null)
        {
            foreach (var permission in permissions)
                dictionary[permission.Path] = permission;
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, options);
    }
}