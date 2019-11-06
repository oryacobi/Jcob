using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace OrYacobi.Jcob.Play
{
    class Program
    {
        static void Main()
        {
            var classToSerielze = new List<TestJsonWithBinary>();
            var classToSerielzeNoConverting = new List<TestJsonWithBinary>();

            for (int i = 0; i < 5000; i++)
            {
                classToSerielze.Add(new TestJsonWithBinary(true));
                classToSerielzeNoConverting.Add(new TestJsonWithBinary(true));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            var result = JcobConvert.SerializeObject(classToSerielze, Formatting.Indented, new JsonSerializerSettings());


            var deserializedClass = JcobConvert.DeserializeObject<List<TestJsonWithBinary>>(result, new JsonSerializerSettings());
            var timeWithBinary = stopwatch.ElapsedMilliseconds;
            deserializedClass = null;
            int numOfBytesWithBinary = result.Length;
            stopwatch.Restart();
            string jsonResult = JsonConvert.SerializeObject(classToSerielzeNoConverting, Formatting.Indented, new JsonSerializerSettings());
            var deserializedClassNoConverting = JsonConvert.DeserializeObject<List<TestJsonWithBinary>>(jsonResult, new JsonSerializerSettings());
            var timeWithoutBinary = stopwatch.ElapsedMilliseconds;
            int numOfBytesWithoutBinary = Encoding.UTF8.GetBytes(jsonResult).Length;

            Console.WriteLine($"{nameof(timeWithBinary)}: {timeWithBinary}");
            Console.WriteLine($"{nameof(numOfBytesWithBinary)}: {numOfBytesWithBinary}");
            Console.WriteLine($"{nameof(timeWithoutBinary)}: {timeWithoutBinary}");
            Console.WriteLine($"{nameof(numOfBytesWithoutBinary)}: {numOfBytesWithoutBinary}");

        }

        public class TestJsonWithBinary
        {
            public int IntA { get; set; }
            public int IntB { get; set; }
            public int IntC { get; set; }

            [JsonConverter(typeof(JcobConverter))]
            public double[] BytesA { get; set; }

            [JsonConverter(typeof(JcobConverter))]
            public float[] BytesB { get; set; }


            public string StringA { get; set; }

            [JsonConverter(typeof(JcobConverter))]
            public double[] BytesC { get; set; }

            public string StringB { get; set; }

            public double[] BytesD { get; set; }

            public string StringC { get; set; }

            [JsonConverter(typeof(JcobConverter))]
            public double[] BytesE { get; set; }

            public double[] BytesF { get; set; }

            [JsonConverter(typeof(JcobConverter))]
            public byte[] BytesG { get; set; }


            public TestJsonWithBinary()
            {
            }

            public TestJsonWithBinary(bool x)
            {
                IntA = 1;
                IntB = 2;
                IntC = 3;

                BytesA = new double[2000];
                for (int i = 0; i < BytesA.Length; i++)
                {
                    BytesA[i] = i + 0.22665418454213;
                }

                BytesB = new float[2000];
                for (int i = 0; i < BytesB.Length; i++)
                {
                    BytesB[i] = i + 0.25418454213f;
                }
                BytesC = new double[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
                BytesD = new double[] { 4 };
                BytesE = new double[] { };
                BytesF = null;
                BytesG = null;

                StringA = "111111111111";
                StringB = "2";
                StringC = "33333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333 3333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333 3333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333   ";
            }
        }
    }
}
