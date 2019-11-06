using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrYacobi.Jcob.Helpers;
using OrYacobi.Jcob.Models;

namespace OrYacobi.Jcob
{
    public class JcobConverter : JsonConverter
    {
        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        private readonly bool _writeType;

        public static readonly object Locker = new object();


        public static JsonSerializer AddressSerializer;

        static JcobConverter()
        {
            AddressSerializer = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None};
        }

        public JcobConverter()
        {

        }

        public JcobConverter(bool writeType)
        {
            _writeType = writeType;
        }



        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var binaryDataStream = JcobConvert.GetBinaryDataStream(Thread.CurrentThread.ManagedThreadId);

            if (binaryDataStream != null)
            {
                var memoryStream = SerializeFunction(value, out var structure);
                BinaryAddress address = binaryDataStream.AddData(memoryStream, value.GetType(), structure);

                if (_writeType)
                {
                    address.Type = value.GetType();
                }

                var jObject = JObject.FromObject(address, AddressSerializer);
                jObject.WriteTo(writer);
                return;
            }

            var converters = serializer.Converters.Where(x => !(x is JcobConverter)).ToArray();
            var token = JToken.FromObject(value);
            token.WriteTo(writer, converters);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var binaryDataStream = JcobConvert.GetBinaryDataStream(Thread.CurrentThread.ManagedThreadId);

            if (binaryDataStream != null)
            {
                JObject jsonObject = JObject.Load(reader);

                BinaryAddress binaryAddress = jsonObject.ToObject<BinaryAddress>();
                if (binaryAddress.Type != null)
                {
                    objectType = binaryAddress.Type;
                }

                var bytes = binaryDataStream.GetDataBytes(binaryAddress);

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    return DeserializeFunction(memoryStream, objectType, binaryAddress.Structure);
                }
            }

            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSerializable;
        }



        public virtual MemoryStream SerializeFunction(object obj, out int[][] structure)
        {
            MemoryStream memoryStream;
            var type = obj.GetType();

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                if (elementType != null && (elementType.IsPrimitive || elementType == typeof(decimal) ||
                                            (elementType.IsArray && ((elementType.GetElementType()?.IsPrimitive ?? false) || elementType.GetElementType() == typeof(decimal)))))
                {
                    var bytes = TypesConversions.Serielize(obj, out structure);
                    memoryStream = new MemoryStream(bytes);
                    return memoryStream;
                }
            }

            memoryStream = new MemoryStream();
            BinaryFormatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            structure = null;
            return memoryStream;
        }

        public virtual object DeserializeFunction(MemoryStream memoryStream, Type type, int[][] structure)
        {
            if (structure != null)
            {
                var result = TypesConversions.Deserielize(memoryStream.ToArray(), type, structure);
                return result;
            }

            return BinaryFormatter.Deserialize(memoryStream);
        }
    }
}
