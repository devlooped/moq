using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.IdentityModel.Tokens;

namespace Devlooped.Sponsors;

static partial class JsonOptions
{
    public static JsonSerializerOptions Default { get; } =
#if NET6_0_OR_GREATER
        new(JsonSerializerDefaults.Web)
#else
        new()
#endif
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
#if NET6_0_OR_GREATER
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull,
#endif
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(allowIntegerValues: false),
#if NET6_0_OR_GREATER
                new DateOnlyJsonConverter()
#endif
            }
        };

    public static JsonSerializerOptions JsonWebKey { get; } = new(JsonSerializerOptions.Default)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
                {
                    info =>
                    {
                        if (info.Type != typeof(JsonWebKey))
                            return;

                        foreach (var prop in info.Properties)
                        {
                            // Don't serialize empty lists, makes for more concise JWKs
                            prop.ShouldSerialize = (obj, value) =>
                                value is not null &&
                                (value is not IList<string> list || list.Count > 0);
                        }
                    }
                }
        }
    };


#if NET6_0_OR_GREATER
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.Parse(reader.GetString()?[..10] ?? "", CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
    }
#endif
}
