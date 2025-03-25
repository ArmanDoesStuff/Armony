#if ARMONY_SERIALIZATION
using Newtonsoft.Json;
using UnityEngine;
using System;

namespace Armony.Utilities.Serialization.Converters
{
    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) return;
            Color color = (Color)value;
            writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA(color)}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string colorString = (string)reader.Value;
            Color color;
            ColorUtility.TryParseHtmlString(colorString, out color);
            return color;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}
#endif