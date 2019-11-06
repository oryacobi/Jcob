using System;
using System.IO;

namespace OrYacobi.Jcob.Helpers
{
    internal static class Packaging
    {
        private const int headerLength = sizeof(long)/sizeof(byte);
        public static MemoryStream CreatePackage(byte[] json, MemoryStream binaryData)
        {
            long jsonLength = json.Length;
            byte[] jsonLengthBytes = BitConverter.GetBytes(jsonLength);
            MemoryStream result = new MemoryStream();
            result.Write(jsonLengthBytes, 0, jsonLengthBytes.Length);
            result.Write(json, 0,json.Length);
            binaryData.CopyTo(result);
            return result;
        }

        public static bool UnpackPackage(MemoryStream inputStream, out byte[] json, out MemoryStream binaryData)
        {
            byte[] jsonLengthBytes = new byte[headerLength];
            inputStream.Read(jsonLengthBytes, 0, headerLength);
            long jsonLength = BitConverter.ToInt64(jsonLengthBytes, 0);

            json = new byte[jsonLength];
            inputStream.Read(json, 0, (int)jsonLength);
            binaryData = inputStream;
            return true;
        }
    }
}
