using System;
using System.IO;

namespace OrYacobi.Jcob.Models
{
    internal class BinaryDataStream : IDisposable
    {
        private readonly MemoryStream _memoryStream;
        private readonly long _initialPosition;
        public BinaryDataStream()
        {
            _initialPosition = 0;
            _memoryStream = new MemoryStream();
        }

        public BinaryDataStream(MemoryStream memoryStream)
        {
            if (memoryStream == null)
            {
                _initialPosition = 0;
                _memoryStream = new MemoryStream();
            }
            else
            {
                _memoryStream = memoryStream;
                _initialPosition = memoryStream.Position;
            }
        }

        public BinaryAddress AddData(MemoryStream binaryStream,Type type, int[][] structure)
        {
            BinaryAddress address = new BinaryAddress { Ptr = _memoryStream.Position - _initialPosition, Len = binaryStream.Length , Structure = structure};
            if (structure != null)
            {
                address.Type = type;
            }
            binaryStream.WriteTo(_memoryStream);
            return address;
        }


        public MemoryStream GetData(BinaryAddress address)
        {
            return new MemoryStream(GetDataBytes(address));
        }

        public byte[] GetDataBytes(BinaryAddress address)
        {
            var result = new byte[address.Len];
            _memoryStream.Position = _initialPosition + address.Ptr;
            _memoryStream.Read(result, 0, (int)address.Len);
            return result;
        }



        public MemoryStream GetAllStream()
        {
            _memoryStream.Position = _initialPosition;
            return _memoryStream;
        }

        public void Dispose()
        {
            _memoryStream?.Dispose();
        }
    }
}

