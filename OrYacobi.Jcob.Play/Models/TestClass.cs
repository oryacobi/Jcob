using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Accord;
using Accord.IO;
using Accord.Math;
using Newtonsoft.Json;
using ProtoBuf;

namespace OrYacobi.Jcob.Play.Models
{
    [ProtoContract]
    public class TestClass
    {
        [ProtoMember(1)]
        public int IntA { get; set; }
        [ProtoMember(2)]
        public int IntB { get; set; }
        [ProtoMember(3)]
        public int IntC { get; set; }

        [ProtoMember(4)]
        public string StringA { get; set; }

        [ProtoMember(5)]
        public string StringB { get; set; }

        [ProtoMember(6)]
        public string StringC { get; set; }

        [ProtoMember(7)]
        public ArraysPair<double> DoubleArrays { get; set; }

        [ProtoMember(8)]
        public ArraysPair<float> FloatArrays { get; set; }

        [ProtoMember(9)]
        public ArraysPair<int> IntArrays { get; set; }

        public TestClass()
        {

        }

        public TestClass(int numberOfElements, double binaryRatio)
        {
            IntA = 1;
            IntB = -2222;
            IntC = 33333333;

            StringA = "1";
            StringB = null;
            StringC =
                "33333333 3333 3 3 3 33333 33333333 3333 3 3 3 33333 33333333 3333 3 3 3 33333 33333333 3333 3 3 3 3333333333333 3333 3 3 3 3333333333333 3333 3 3 3 33333 33333333 3333 3 3 3 3333333333333 3333 3 3 3 3333333333333 3333 3 3 3 33333 33333333 3333 3 3 3 33333";

            DoubleArrays = new ArraysPair<double>(numberOfElements, binaryRatio);
            FloatArrays = new ArraysPair<float>(numberOfElements, binaryRatio);
            IntArrays = new ArraysPair<int>(numberOfElements, binaryRatio);
        }
    }

    [ProtoContract]
    public class ArraysPair<T> 
    {
        [ProtoMember(1)]
        public T[] NonBinaryArray { get; set; }

        [ProtoMember(2)]
        [JsonConverter(typeof(JcobConverter))]
        public T[] BinaryArray { get; set; }

        public ArraysPair()
        {

        }

        public ArraysPair(int numberOfElements, double binaryRatio)
        {
            int numOfBinaryElements = (int)(numberOfElements * binaryRatio);
            int numOfNonBinaryElements = numberOfElements - numOfBinaryElements;

            BinaryArray = GetArray(numOfBinaryElements);
            NonBinaryArray = GetArray(numOfNonBinaryElements);
        }

        public static T[] GetArray(int numberOfElements)
        {
            Type type = typeof(T);
            object[] result = new object[numberOfElements];
            if (result.Length == 0) return new T[0];
            Type baseType = type.IsArray ? type.GetElementType() : type;

            if (type.IsArray)
            {
                for (var index = 0; index < result.Length; index++)
                {
                        MethodInfo method = typeof(ArraysPair<T>).GetMethod("GetPrimitiveArray");
                    if (method != null)
                    {
                        MethodInfo genericMethod = method.MakeGenericMethod(baseType);
                        result[index] =  genericMethod.Invoke(null, new object[] { numberOfElements });
                        continue;
                    }
                    throw new FormatException("Could not find Accord's 'Load' method");
                }

               return result.Cast<T>().ToArray();
            }
            return GetPrimitiveArray<T>(numberOfElements);
        }

        static readonly Dictionary<Type, dynamic>  ValueSteps = new Dictionary<Type, dynamic>
        {
            {typeof(double), 0.1649582384 },
            {typeof(float), -0.162384f },
            {typeof(int), 3 },
        };

        public static TArray[] GetPrimitiveArray<TArray>(int numberOfElements)
        {
            Type type = typeof(TArray);
            dynamic[] result = new dynamic[numberOfElements];

            if (result.Length == 0) return new TArray[0];

            result[0] = ValueSteps[type];

            for (var index = 1; index < result.Length; index++)
            {
                //MethodInfo mi = type.GetMethod("op_Addition",
                //    BindingFlags.Static | BindingFlags.Public);

                //if(mi == null) throw new FormatException();

                result[index] = result[index - 1] + ValueSteps[type];
                //result[index] =  mi.Invoke(null, new [] { result[index - 1], ValueSteps[type]});
            }

            return result.Cast<TArray>().ToArray();
        }
    }
}
