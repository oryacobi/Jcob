using System;

namespace OrYacobi.Jcob.Helpers
{
    public static class TypesConversions
    {
        public static byte[] Serielize(object obj, out int[][] structure)
        {
            var type = obj.GetType();
            if (type.IsArray && type.GetArrayBesicType().IsPrimitive)
                return ((Array)obj).ToBytesArray(out structure);

            throw new ArgumentException($"Unsupported Type '{type.FullName}'");
        }

        public static object Deserielize(byte[] bytes, Type type, int[][] structure)
        {
            if(type.IsArray && type.GetArrayBesicType().IsPrimitive )
                return bytes.ToTypedArray(type, structure);
          
            throw new ArgumentException($"Unsupported Type '{type.FullName}'");
        }

        public static object ConvertToOneDimension<T>(byte[] bytes) where T: struct 
        {
            var result = new T[bytes.Length / PrimitiveSizeHelper.SizeOf(typeof(T))];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }

        public static object ConvertToJaggedArray(byte[] bytes, Type type, int firstDimensionSize)
        {
            Array result = InitializeJaggedArray(typeof(double[]), 0, new[] {firstDimensionSize, firstDimensionSize == 0 ? 0 : bytes.Length / PrimitiveSizeHelper.SizeOf(type) / firstDimensionSize});

           // Array result =  Array.CreateInstance(type, firstDimensionSize, firstDimensionSize == 0 ? 0 : bytes.Length / SizeHelper.SizeOf(type) / firstDimensionSize);
            var sourceOffSet = 0;
            for (var index = 0; index < result.Length; index++)
            {
              //  result.SetValue(Array.CreateInstance(type, new[] { bytes.Length / SizeHelper.SizeOf(type) / firstDimensionSize }), new[] { index });
                Buffer.BlockCopy(bytes, sourceOffSet, (Array)result.GetValue(new[] { index }), 0, bytes.Length / firstDimensionSize);
                sourceOffSet += bytes.Length / firstDimensionSize;
            }
            return result;
        }

        static Array InitializeJaggedArray(Type type, int index, int[] lengths)
        {
            Array array = Array.CreateInstance(type, lengths[index]);
            Type elementType = type.GetElementType();

            if (elementType != null)
            {
                for (int i = 0; i < lengths[index]; i++)
                {
                    array.SetValue(
                        InitializeJaggedArray(elementType, index + 1, lengths), i);
                }
            }

            return array;
        }

 

    }


}
