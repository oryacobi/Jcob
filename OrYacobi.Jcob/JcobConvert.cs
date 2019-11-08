using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrYacobi.Jcob.Helpers;
using OrYacobi.Jcob.Models;

namespace OrYacobi.Jcob
{
    public static class JcobConvert
    {
        private static readonly Dictionary<int, BinaryDataStream> BinaryDataStreams = new Dictionary<int, BinaryDataStream>();

        internal static BinaryDataStream GetBinaryDataStream(int threadId)
        {
            lock (BinaryDataStreams)
            {
                if (!BinaryDataStreams.TryGetValue(threadId, out var binaryDataStream))
                {
                    return null;
                }
                return binaryDataStream;
            }
        }

        private static void SetBinaryDataStream(int threadId, MemoryStream binaryData = null)
        {
            lock (BinaryDataStreams)
            {
                if (BinaryDataStreams.ContainsKey(threadId))
                {
                throw new FormatException($"Binary data steam for thread '{threadId}' already exists");
                }

                BinaryDataStreams[threadId] = new BinaryDataStream(binaryData);
            }
        }

        private static void RemoveBinaryDataStream(int threadId)
        {
            lock (BinaryDataStreams)
            {
                if (!BinaryDataStreams.TryGetValue(threadId, out var stream))
                {
                    return;
                }

                stream.Dispose();
                BinaryDataStreams.Remove(threadId);
            }
        }

        public static byte[] SerializeObject(object value)
        {
            return SerializeObject(value, Formatting.None, new JsonSerializerSettings());
        }

        public static byte[] SerializeObject(object value, Formatting formatting)
        {
            return SerializeObject(value, formatting, new JsonSerializerSettings());
        }


        public static byte[] SerializeObject(object value, Formatting formatting, JsonSerializerSettings settings)
        {
            using (MemoryStream memoryStream = SerializeObjectToStream(value, formatting, settings))
            {
                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> SerializeObjectAsync(object value, Formatting formatting, JsonSerializerSettings settings)
        {
            var result = await Task.Run(() => SerializeObject(value, formatting, settings));
            return result;
        }

        public static MemoryStream SerializeObjectToStream(object value, Formatting formatting, JsonSerializerSettings settings)
        {
            try
            {
                SetBinaryDataStream(Thread.CurrentThread.ManagedThreadId);

                var jsonPart = JsonConvert.SerializeObject(value, formatting, settings);
                var result = Packaging.CreatePackage(Encoding.UTF8.GetBytes(jsonPart), GetBinaryDataStream(Thread.CurrentThread.ManagedThreadId).GetAllStream());
                return result;
            }
            finally
            {
                RemoveBinaryDataStream(Thread.CurrentThread.ManagedThreadId);
            }
        }

        public static T DeserializeObject<T>(byte[] bytes)
        {
            return DeserializeObjectFromStream<T>(new MemoryStream(bytes), new JsonSerializerSettings());
        }

        public static T DeserializeObject<T>(byte[] bytes, JsonSerializerSettings settings)
        {
            return DeserializeObjectFromStream<T>(new MemoryStream(bytes), settings);
        }

        public static T DeserializeObjectFromStream<T>(MemoryStream input, JsonSerializerSettings settings)
        {

                Packaging.UnpackPackage(input, out byte[] json, out MemoryStream binaryData);

            try
            {
                SetBinaryDataStream(Thread.CurrentThread.ManagedThreadId, binaryData);
                var result = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json), settings);
                return result;
            }
            finally
            {
                RemoveBinaryDataStream(Thread.CurrentThread.ManagedThreadId);

            }
        
        }

        //private static bool CanConvertFunc(Type type)
        //{
        //    return type.IsSerializable;
        //}

        //private static object DeserializeFunc(BinaryAddress binaryAddress, Type type)
        //{
        //    var bytes = GetBinaryDataStream(Thread.CurrentThread.ManagedThreadId).GetDataBytes(binaryAddress);

        //    if (DeserializeExtensionFunc != null)
        //    {
        //        var result = DeserializeExtensionFunc.Invoke(bytes, type, binaryAddress.Structure);
        //        if (result != null)
        //        {
        //            return result;
        //        }
        //    }

        //    if (binaryAddress.Structure != null)
        //    {
        //        var result = TypesConversions.Deserielize(bytes, type, binaryAddress.Structure);
        //        return result;
        //    }

        //    var obj = BinaryFormatter.Deserialize(new MemoryStream(bytes));
        //    return obj;
        //}

        //private static BinaryAddress SerializeFunc(object obj, Type type)
        //{
        //    int[][] structure = null;
        //    MemoryStream memoryStream = null;
        //    try
        //    {
        //        bool usedExtension = false;
        //        if (SerializeExtensionFunc != null)
        //        {
        //            var bytes = SerializeExtensionFunc.Invoke(obj, out structure);
        //            if (bytes != null)
        //            {
        //                usedExtension = true;
        //                memoryStream = new MemoryStream(bytes);
        //            }
        //        }

        //        if (!usedExtension)
        //        {
        //            if (type.IsArray)
        //            {
        //                Type elementType = type.GetElementType();
        //                if (elementType != null && (elementType.IsPrimitive || elementType == typeof(decimal) ||
        //                                            (elementType.IsArray && ((elementType.GetElementType()?.IsPrimitive ?? false) || elementType.GetElementType() == typeof(decimal)))))
        //                {
        //                    var bytes = TypesConversions.Serielize(obj, out structure);
        //                    memoryStream = new MemoryStream(bytes);
        //                    return GetBinaryDataStream(Thread.CurrentThread.ManagedThreadId).AddData(memoryStream, type, structure);
        //                }
        //            }

        //            memoryStream = new MemoryStream();
                  
        //            memoryStream.Position = 0;
        //        }

        //        return GetBinaryDataStream(Thread.CurrentThread.ManagedThreadId).AddData(memoryStream, type, structure);
        //    }
        //    finally
        //    {
        //        memoryStream?.Dispose();
        //    }
        //}
    }
}
