using System;
using System.Collections.Generic;
using System.Linq;

namespace OrYacobi.Jcob.Helpers
{
    static class ArrayExtensions
    {
        public static void InitValues<T>(this T[,] array, T value)
        {
            for (var index0 = 0; index0 < array.GetLength(0); index0++)
            for (var index1 = 0; index1 < array.GetLength(1); index1++)
            {
                array[index0, index1] = value;
            }
        }

        public static void InitValues<T>(this T[] array, T value)
        {
            for (var index = 0; index < array.Length; index++)
            {
                array[index] = value;
            }
        }

        public static T[] GetSubArray<T>(this T[] originalArray, int[] indexes)
        {
            T[] result = new T[indexes.Length];

            var newIndex = 0;
            foreach (int index in indexes)
            {
                result[newIndex++] = originalArray[index];
            }

            return result;
        }

        public static T[,] GetSubArray<T>(this T[,] originalArray, int[] indexes0, int[] indexes1)
        {
            T[,] result = new T[indexes0.Length, indexes1.Length];
            var newIndex0 = 0;
            foreach (int index0 in indexes0)
            {
                var newIndex1 = 0;
                foreach (int index1 in indexes1)
                {
                    result[newIndex0, newIndex1++] = originalArray[index0, index1];
                }

                newIndex0++;
            }

            return result;
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate.Invoke(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static IEnumerable<TSource> TryTake<TSource>(this IEnumerable<TSource> source, int numberOfElements)
        {
            return source.TakeWhile((x, index) => index < numberOfElements);
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public static IEnumerable<TK> Select<TK, T>(this IEnumerator<T> e, Func<T, TK> selector)
        {
            while (e.MoveNext())
            {
                yield return selector(e.Current);
            }
        }

        public static TResult[,] Select<TArray, TResult>(this TArray[,] array, Func<TArray, TResult> selector)
        {
            if (array == null) throw new ArgumentException(nameof(array));

            var length0 = array.GetLength(0);
            var length1 = array.GetLength(1);
            var result = new TResult[length0, length1];

            for (var index0 = 0; index0 < length0; index0++)
            for (var index1 = 0; index1 < length1; index1++)
            {
                result[index0, index1] = selector.Invoke(array[index0, index1]);
            }

            return result;
        }

        public static bool IsJaggedArray(this Type type)
        {
            return typeof(Array).IsAssignableFrom(type);
        }

        public static int[][] GetArrayStructure(this Array array, out Type basicType) //[Jagged level][Dimenstions]
        {
            basicType = array.GetArrayBesicType();
            int[] lastDimenstions = array.GetArrayDimenstions();
            List<int[]> length = new List<int[]>() {lastDimenstions};

            Type elementType = array.GetType().GetElementType();

            while ((elementType?.IsArray ?? false) && lastDimenstions.Any() && lastDimenstions.All(x => x > 0))
            {

                array = (Array) array.GetValue(new int[lastDimenstions.Length]);
                lastDimenstions = array.GetArrayDimenstions();
                length.Add(lastDimenstions);
                elementType = array.GetType().GetElementType();
            }

            return length.ToArray();
        }

        public static int[] GetArrayDimenstions(this Array array)
        {
            List<int> lengths = new List<int>();
            for (int dim = 0; dim < array.Rank; dim++)
            {
                lengths.Add(array.GetLength(dim));
            }

            return lengths.ToArray();
        }

        public static T[,] To2DArray<T>(this T[][] source)
        {
            if (source.Length == 0) return new T[0, 0];
            var subArrayLenght = source[0].Length;
            T[,] result = new T[source.Length, subArrayLenght];

            for (int i = 0; i < source.Length; i++)
            {
                if (subArrayLenght != source[i].Length)
                {
                    throw new ArgumentException("The given jagged array is not rectangular.");
                }

                for (int k = 0; k < source[0].Length; k++)
                {
                    result[i, k] = source[i][k];
                }
            }

            return result;
        }

        public static T[][] ToJaggedArray<T>(this T[,] source)
        {
            if (source.Length == 0) return new T[0][];

            T[][] result = new T[source.GetLength(0)][];

            int arraysLength = source.GetLength(1);
            for (int i = 0; i < source.GetLength(0); i++)
            {
                T[] array = new T[arraysLength];

                for (int k = 0; k < arraysLength; k++)
                {
                    array[k] = source[i, k];
                }

                result[i] = array;
            }

            return result;
        }

        public static T[,] ToArray<T>(this T[,] source)
        {
            if (source.Length == 0) return new T[0, 0];
            T[,] result = new T[source.GetLength(0), source.GetLength(1)];

            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int k = 0; k < source.GetLength(1); k++)
                {
                    result[i, k] = source[i, k];
                }
            }

            return result;
        }




        private static int GetNumberOfArrayIdenticalParts(int[][] structure)
        {
            int numberOfArrayParts = 1;
            for (int jaggedTempIndex = 0; jaggedTempIndex < structure.Length - 1; jaggedTempIndex++)
            {
                numberOfArrayParts *= structure[jaggedTempIndex].Sum();
            }

            return numberOfArrayParts;
        }

        private static int GetNumberOfArrayElements(int[][] structure)
        {
            int numOfElements = GetNumberOfArrayIdenticalParts(structure);
            foreach (var dim in structure.Last())
            {
                numOfElements *= dim;
            }

            return numOfElements;
        }

        public static Type GetArrayBesicType(this Array array)
        {
            return GetArrayBesicType(array.GetType());
        }

        public static Type GetArrayBesicType(this Type arrayType)
        {
            if (!arrayType.IsArray) throw new ArgumentException(nameof(arrayType));
            Type elementType = arrayType.GetElementType();
            if (elementType == null) throw new FormatException("Invalid array. First element type cannot be null");
            while (true)
            {
                var nextElementType = elementType.GetElementType();
                if (nextElementType == null) return elementType;
                elementType = nextElementType;
            }
        }


        public static Array InitializeMultidimensionalArray(Type type, int[] lengths)
        {
            return InitializeArray(type, 0, new[] {lengths});
        }

        public static Array InitializeJaggedArray(Type type, int[] lengths)
        {
            return InitializeArray(type, 0, lengths.Select(x => new[] {x}).ToArray());
        }

        public static Array InitializeArray(Type type, int[][] lengths)
        {
            return InitializeArray(type, 0, lengths);
        }

        private static Array InitializeArray(Type type, int jaggedIndex, int[][] structure)
        {
            int x = 0;
            return InitializeArrayFromBytes(type, jaggedIndex, structure, null, ref x);
        }

        private static Array InitializeArrayFromBytes(Type type, int jaggedIndex, int[][] structure, byte[] input, ref int inputOffset) //[Jagged level][Dimenstions]
        {
            Type elementType = type.GetElementType();
            if (elementType == null) throw new FormatException("Invalid array. First element type cannot be null");

            Array array = Array.CreateInstance(elementType, structure[jaggedIndex]);

            if (elementType.GetElementType() == null || structure[jaggedIndex].Length != 1 || jaggedIndex == structure.Length - 1)
            {
                if (input != null) //Copy Bytes
                {
                    int numberOfArrayParts = GetNumberOfArrayIdenticalParts(structure);
                    int bytesTocopy = input.Length / numberOfArrayParts;
                    Buffer.BlockCopy(input, inputOffset, array, 0, bytesTocopy);
                    inputOffset += bytesTocopy;
                }

                return array;
            }

            if (structure[jaggedIndex].Length != 1) throw new FormatException("Unsupported array type");
            for (int i = 0; i < structure[jaggedIndex].First(); i++)
            {
                array.SetValue(InitializeArrayFromBytes(elementType, jaggedIndex + 1, structure, input, ref inputOffset), i);
            }

            return array;
        }


        public static Array ToTypedArray(this byte[] inputBytes, Type type, int[][] structure) //[Jagged level][Dimenstions]
        {
            int jaggedIndex = 0;
            return InitializeArrayFromBytes(type, 0, structure, inputBytes, ref jaggedIndex);
        }


        public static byte[] ToBytesArray(this Array array, out int[][] structure)
        {
            Type baseType = GetArrayBesicType(array);
            if (!baseType.IsPrimitive && baseType != typeof(decimal))
            {
                throw new ArgumentException($"This function supports primitive and decimal types only. (Provided base type was '{baseType}')");
            }

            if (baseType == typeof(decimal))
            {
                throw new NotImplementedException("oh decimals...");
            }

            int sizeOfBaseType = PrimitiveSizeHelper.SizeOf(baseType);
            int multiplyFactor = sizeOfBaseType / sizeof(byte);
            structure = array.GetArrayStructure(out _);
            byte[] bytesResult = new byte[GetNumberOfArrayElements(structure) * multiplyFactor];
            int bytesOffset = 0;
            ToBytesArray(array, multiplyFactor, structure, 0, ref bytesResult, ref bytesOffset);
            return bytesResult;
        }

        private static void ToBytesArray(this Array array, int multiplyFactor, int[][] structure, int jaggedIndex, ref byte[] bytesResult, ref int bytesOffset)
        {
            Type elementType = array.GetType().GetElementType();
            if (elementType == null) throw new FormatException("Invalid array. First element type cannot be null");

            elementType = elementType.GetElementType();

            if (elementType == null || structure[jaggedIndex].Length != 1 || jaggedIndex == structure.Length - 1)
            {
                int bytesTocopy = array.Length * multiplyFactor;
                Buffer.BlockCopy(array, 0, bytesResult, bytesOffset, bytesTocopy);
                bytesOffset += bytesTocopy;
                return;
            }

            if (structure[jaggedIndex].Length != 1) throw new FormatException("Unsupported array type");
            for (int i = 0; i < structure[jaggedIndex].First(); i++)
            {
                ToBytesArray((Array) array.GetValue(i), multiplyFactor, structure, jaggedIndex + 1, ref bytesResult, ref bytesOffset);
            }
        }

    }
}