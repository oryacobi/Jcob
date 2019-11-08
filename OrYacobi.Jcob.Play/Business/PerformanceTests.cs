using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OrYacobi.Jcob.Play.Helpers;
using OrYacobi.Jcob.Play.Models;

namespace OrYacobi.Jcob.Play.Business
{
    public class PerformanceTests
    {
        public IEnumerable<PerformanceResults> Run(IEnumerable<SerializationConfig> configs = null)
        {
            if (configs == null)
            {
                configs = Enum.GetValues(typeof(SerializationConfig)).Cast<SerializationConfig>();
            }

            foreach (SerializationConfig serializationConfig in configs)
            {
                for (double ratio = 0; ratio <= 1.00001; ratio += 0.02)
                {
                    var performanceResult = new PerformanceResults {SerializingConfig = serializationConfig, BinaryDataRatio = ratio};

                    var testClass = new TestClass(performanceResult.NumberOfArrayElements, performanceResult.BinaryDataRatio);
                    byte[] serializedBytes = null;
                    string serializedString = null;

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < performanceResult.Iterations; i++)
                    {
                        switch (serializationConfig)
                        {
                            case SerializationConfig.Json:
                                serializedString =JsonConvert.SerializeObject(testClass);
                                break;
                            case SerializationConfig.Jcob:
                                serializedBytes = JcobConvert.SerializeObject(testClass);
                                break;
                            case SerializationConfig.Protobuf:
                                serializedBytes = testClass.ProtoSerialize();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    performanceResult.SerializationMs = stopwatch.ElapsedMilliseconds;

                    if (serializedString != null)
                    {
                        serializedBytes = Encoding.UTF8.GetBytes(serializedString);
                    }

                    performanceResult.Size = serializedBytes?.Length ?? -1;
                     File.WriteAllBytes("SerializedTestClass.txt", serializedBytes);


                    stopwatch.Restart();
                    TestClass deserializedClass = null;
                    for (int i = 0; i < performanceResult.Iterations; i++)
                    {
                        switch (serializationConfig)
                        {
                            case SerializationConfig.Json:
                                deserializedClass =  JsonConvert.DeserializeObject<TestClass>(serializedString);
                                break;
                            case SerializationConfig.Jcob:
                                deserializedClass = JcobConvert.DeserializeObject<TestClass>(serializedBytes);
                                break;
                            case SerializationConfig.Protobuf:
                                deserializedClass = serializedBytes.ProtoDeserialize<TestClass>();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    performanceResult.DeserializationMs = stopwatch.ElapsedMilliseconds;

                    if (deserializedClass == null)
                    {
                        throw new FormatException();
                    }

                    yield return performanceResult;

                    if(serializationConfig != SerializationConfig.Jcob) break;
                }

            }
        }

        public class PerformanceResults
        {
            public SerializationConfig SerializingConfig { get; set; }
            public int Iterations { get; set; } = 1500;
            public int NumberOfArrayElements { get; set; } = 2000;
            public double BinaryDataRatio { get; set; }

            public long SerializationMs { get; set; } = -1;
            public long DeserializationMs { get; set; } = -1;
            public long TotalMs => SerializationMs + DeserializationMs;
            public long Size { get; set; } = -1;

            public override string ToString()
            {
                return $"{SerializingConfig} {Iterations} {NumberOfArrayElements} {BinaryDataRatio} {SerializationMs} {DeserializationMs} {TotalMs} {Size}";
            }
        }

     

        public enum SerializationConfig
        {
            Protobuf,
            Jcob,
            Json,
        }
    }
}
