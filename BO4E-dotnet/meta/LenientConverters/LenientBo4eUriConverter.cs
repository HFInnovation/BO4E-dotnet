﻿using Newtonsoft.Json;

using System;

namespace BO4E.meta.LenientConverters
{
    /// <summary>
    /// allows deserializing <see cref="Bo4EUri"/>s
    /// </summary>
    public class LenientBo4EUriConverter : JsonConverter
    {
        /// <inheritdoc cref="JsonConverter.CanConvert(Type)"/>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Bo4EUri));
        }

        /// <inheritdoc cref="JsonConverter.ReadJson(JsonReader, Type, object, JsonSerializer)"/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }
            var rawString = (string)reader.Value;
            if (rawString.Trim() == String.Empty)
            {
                return null;
            }
            return new Bo4EUri(rawString);
        }

        /// <inheritdoc cref="JsonConverter.CanConvert(Type)"/>
        public override bool CanWrite => false;

        /// <inheritdoc cref="JsonConverter.WriteJson(JsonWriter, object, JsonSerializer)"/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}