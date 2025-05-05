#if ARMONY_SERIALIZATION
using Newtonsoft.Json;
using UnityEngine;
using System;

namespace Armony.Utilities.Serialization.Converters
{
    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            if (_value == null) return;
            Color color = (Color)_value;
            _writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA(color)}");
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            string colorString = (string)_reader.Value;
            ColorUtility.TryParseHtmlString(colorString, out Color color);
            return color;
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(Color);
        }
    }
}
#endif