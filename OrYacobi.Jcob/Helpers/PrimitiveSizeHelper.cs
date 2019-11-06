using System;
using System.Collections.Generic;
using System.Linq;

namespace OrYacobi.Jcob.Helpers
{
    public static class PrimitiveSizeHelper
    {
        private static readonly Dictionary<Type, int> SizesCache;

        static PrimitiveSizeHelper()
        {
            SizesCache = new Dictionary<Type, int>
            {
                {typeof(byte), sizeof(byte)},
                {typeof(sbyte), sizeof(sbyte)},
                {typeof(decimal), sizeof(decimal)}
            };


            foreach (var type in typeof(int).Assembly.GetTypes().Where(t => t.IsPrimitive && t != typeof(IntPtr) && t != typeof(UIntPtr)))
            {
                if(SizesCache.ContainsKey(type)) continue;

                var result = (byte[]) typeof(BitConverter).GetMethod("GetBytes", new[] {type})?.Invoke(null, new[] {Activator.CreateInstance(type)});
                if (result == null)
                {
                    throw new FormatException($"Unsupported type '{type.Name}'");
                }

                SizesCache.Add(type, result.Length);
            }
        }

        public static int SizeOf(Type type)
        {
            if (SizesCache.TryGetValue(type, out int size))
            {
                return size;
            }

            throw new ArgumentOutOfRangeException($"This function does not support the type '{type.Name}'. It probably does not have a constant size..");
        }

    }
}
