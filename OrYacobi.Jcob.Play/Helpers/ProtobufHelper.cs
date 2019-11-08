using System.IO;
using ProtoBuf;

namespace OrYacobi.Jcob.Play.Helpers
{
    public static class ProtobufHelper
    {
        /// <summary>
        /// Serialize using Protobuf. from https://www.c-sharpcorner.com/article/serialization-and-deserialization-ib-c-sharp-using-protobuf-dll/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        public static byte[] ProtoSerialize<T>(this T record) where T : class
        {
            if (null == record) return null;

            try
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, record);
                    return stream.ToArray();
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }

        /// <summary>
        /// Deerialize using Protobuf. 
        /// </summary>
        /// <returns></returns>
        public static T ProtoDeserialize<T>(this  byte[] bytes) where T : class
        {
            if (null == bytes) return null;

            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    return Serializer.Deserialize<T>(stream);
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }
    }
}
