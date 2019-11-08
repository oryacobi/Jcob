using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OrYacobi.Jcob.Play.Business;
using OrYacobi.Jcob.Play.Models;

namespace OrYacobi.Jcob.Play
{
    class Program
    {
        static void Main()
        {

            PerformanceTests test  = new PerformanceTests();
            var performanceResults = test.Run();

            foreach (PerformanceTests.PerformanceResults result in performanceResults)
            {
                Console.WriteLine(result);
            }


            // ExampleClass originalObject = new ExampleClass();

            //// byte[] jcobBytes = JcobConvert.SerializeObject(originalObject);

            // string jsonString = JsonConvert.SerializeObject(originalObject);


            //// ExampleClass deserializedObject = JcobConvert.DeserializeObject<ExampleClass>(jcobBytes);


            // ExampleAccord accordExample = new ExampleAccord();
            // byte[] jcobBytes = JcobConvert.SerializeObject(accordExample);
            // ExampleAccord deserializedObject = JcobConvert.DeserializeObject<ExampleAccord>(jcobBytes);
            // File.WriteAllBytes("Jcob.txt", jcobBytes);





            //var classToSerielze = new List<TestClass>();
            //var classToSerielzeNoConverting = new List<TestClass>();

            //for (int i = 0; i < 5000; i++)
            //{
            //    classToSerielze.Add(new TestClass(true));
            //    classToSerielzeNoConverting.Add(new TestClass(true));
            //}

            //Stopwatch stopwatch = Stopwatch.StartNew();

            //var result = JcobConvert.SerializeObject(classToSerielze, Formatting.Indented, new JsonSerializerSettings());


            //var deserializedClass = JcobConvert.DeserializeObject<List<TestClass>>(result, new JsonSerializerSettings());
            //var timeWithBinary = stopwatch.ElapsedMilliseconds;
            //deserializedClass = null;
            //int numOfBytesWithBinary = result.Length;
            //stopwatch.Restart();
            //string jsonResult = JsonConvert.SerializeObject(classToSerielzeNoConverting, Formatting.Indented, new JsonSerializerSettings());
            //var deserializedClassNoConverting = JsonConvert.DeserializeObject<List<TestClass>>(jsonResult, new JsonSerializerSettings());
            //var timeWithoutBinary = stopwatch.ElapsedMilliseconds;
            //int numOfBytesWithoutBinary = Encoding.UTF8.GetBytes(jsonResult).Length;

            //Console.WriteLine($"{nameof(timeWithBinary)}: {timeWithBinary}");
            //Console.WriteLine($"{nameof(numOfBytesWithBinary)}: {numOfBytesWithBinary}");
            //Console.WriteLine($"{nameof(timeWithoutBinary)}: {timeWithoutBinary}");
            //Console.WriteLine($"{nameof(numOfBytesWithoutBinary)}: {numOfBytesWithoutBinary}");

             Console.ReadLine();

        }




    }
}
