# Jcob
Jcob is an extension to Json.NET which allows you to integrate binary content into a Json format (efficiently!)

For example, this Json structure:
```json
{
  "FirstName": "James",
  "MiddleName": "Batman",
  "LastName": "Newton-King",
  "Age": -1,
  "ScoresArray": [
    1.6,
    18.03,
    3.0,
    9.8,
    8.7,
    4.989,
    48.48,
    2.045,
    8.68,
    343.656,
    3.811,
    77.2,
    0.3091,
    798.0,
    0.57628,
    6.2,
    1.3,
    54.0,
    48.622,
    70.52,
    60.4628
  ]
}
```

Turns into this Jcob:
```json
[Binary_Prefix]{
  "FirstName": "James",
  "MiddleName": "Batman",
  "LastName": "Newton-King",
  "Age": -1,
  "ScoresArray": {
    "Ptr": 0,
    "Len": 168,
    "Structure": [[21]]
  }
}[.............Binary_Content.............]
```

## When to use it?
1. You need to serialize large numeric arrays 
2. You need to integrate a binary serializer result (or any other binary content) in your Json 
3. You like to work with the Json structure
4. You tested other approaches and were disappointed with the results and performance

## When **NOT** to use is?
1. You are **restriced to a valid Json** format 
2. You need to be able to see and understand **all** the content in the serialization result
3. The total amount of binary contant is very small (in this case, consider [this](https://stackoverflow.com/questions/1443158/binary-data-in-json-string-something-better-than-base64))

## Dependencies
.NET Standard 2.0 ([Full dependency table](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support))

## License
[MIT](../master/LICENSE)

## Usage Examples
```csharp
static void Main()
{
    ExampleClass originalObject = new ExampleClass();

    byte[] jcobBytes = JcobConvert.SerializeObject(originalObject);

    ExampleClass deserializedObject = JcobConvert.DeserializeObject<ExampleClass>(jcobBytes);
}

public class ExampleClass
{
    public string FirstName { get; set; } = "James";

    public string MiddleName { get; set; } = "Batman";

    public string LastName { get; set; } = "Newton-King";

    public int Age { get; set; } = -1;

    [JsonConverter(typeof(JcobConverter))]
    public double[] ScoresArray { get; set; } = {1.6, 18.03, 3, 9.8, 8.7, 4.989, 48.48, 2.045, 8.68, 343.656, 3.811, 77.2, 0.3091, 798, 0.57628, 6.2, 1.3, 54, 48.622, 70.52, 60.4628};
}
```

If you want to serialize it to a normal Json, simply use JsonConvert instead: (and the Jcob converter will disable itself)
```csharp
string jsonString = JsonConvert.SerializeObject(originalObject);
```

You can extend the converter usage to additional object types. In the following example we needed to intergate an Accord.Math histogram into the serialization result. 
As long as you know how to **convert your object into bytes, and back from bytes to an object** - it's pretty simple
```csharp
static void Main()
{
    ExampleAccord accordExample = new ExampleAccord();
    byte[] jcobBytes = JcobConvert.SerializeObject(accordExample);
    ExampleAccord deserializedObject = JcobConvert.DeserializeObject<ExampleAccord>(jcobBytes);
}

public class JcobAccordConverter : JcobConverter
{
    public override MemoryStream SerializeFunction(object obj, out int[][] structure)
    {
        Type type = obj.GetType();
        List<string> typesNamespaces = new List<string> { type.Namespace };
        typesNamespaces.AddRange(type.GenericTypeArguments.Select(x => x.Namespace));

        if (typesNamespaces.Any(x => x.IndexOf(nameof(Accord), StringComparison.CurrentCultureIgnoreCase) >= 0))
        {
            //If this type is from Accord namespace, use Accord.IO.Serializer.Save()
            obj.Save(out byte[] bytes);
            structure = null;
            return new MemoryStream(bytes);
        }

        return base.SerializeFunction(obj, out structure);
    }

    public override object DeserializeFunction(MemoryStream memoryStream, Type type, int[][] structure)
    {
        List<string> typesNamespaces = new List<string> { type.Namespace };
        typesNamespaces.AddRange(type.GenericTypeArguments.Select(x => x.Namespace));

        if (typesNamespaces.Any(x => x.IndexOf(nameof(Accord), StringComparison.CurrentCultureIgnoreCase) >= 0))
        {
            //If this type is from Accord namespace, Use Accord.IO.Serializer.Load()
            MethodInfo method = typeof(Accord.IO.Serializer).GetMethod("Load", new Type[] { typeof(byte[]), typeof(SerializerCompression) });
            if (method != null)
            {
                MethodInfo genericMethod = method.MakeGenericMethod(type);
                return genericMethod.Invoke(null, new object[] { memoryStream.ToArray(), SerializerCompression.None });
            }
            throw new FormatException("Could not find Accord's 'Load' method");
        }

        return base.DeserializeFunction(memoryStream, type, structure);
    }
}

public class ExampleAccord
{
    public string NiddleName { get; set; } = "Batman";

    [JsonConverter(typeof(JcobConverter))]
    public double[] ScoresArray { get; set; } = { 1.6, 18.03, 3, 9.8, 8.7 };

    [JsonConverter(typeof(JcobAccordConverter))] //Mind the new converter type
    public ContinuousHistogram AccordHistogram { get; set; } = new ContinuousHistogram(new []{1,2,3}, new Range(0,10));
}
```

## Performance Tests
### Jcob vs Json
The test was performed using this [method](../master/OrYacobi.Jcob.Play/Business/PerformanceTests.cs) and this [data structure](../master/OrYacobi.Jcob.Play/Models/TestClass.cs). Is short, we created a class with several numeric arrays, some with a JcobConverter attribute and some without. By changing the number of elements in each array, we were able to determine the binary ratio in the serialization process. 

The JcobConverter handles numaric arrays simply by copy the relevant area from memory, directly to the serialization binary content. In some cases, this approach may reduce the serialization\deserialization time by 90%+ and the size by 50%.

Binary Data Ratio|Serialization (ms)|Deserialization (ms)|Size (bytes)|Serialization Impovement|Deserialization Improvement|Size Improvement
------------------|------------------|------------------|------------------|------------------|------------------|------------------|
Json|3916|3090|70354|NA|NA|NA
0|4281|3444|70461|-9.32|-11.46|-0.15
0.1|3827|3047|66668|2.27|1.39|5.24
0.2|3409|2723|62875|12.95|11.88|10.63
0.3|2978|2389|58976|23.95|22.69|16.17
0.4|2320|1950|55089|40.76|36.89|21.70
0.5|1936|1619|51196|50.56|47.61|27.23
0.6|1557|1313|47309|60.24|57.51|32.76
0.7|1174|1002|43408|70.02|67.57|38.30
0.8|808|681|39818|79.37|77.96|43.40
0.9|449|374|36190|88.53|87.90|48.56
1|73|57|32628|98.14|98.16|53.62

![Jcob vs Json](../master/Resources/20191109%20Jcob%20vs%20Json%20graph.png)
### Jcob vs Protobuf-net
Jcob has a [small performance advantage](../master/Resources/20191109%20Jcob%20Perfotmance%20Tests%20vs%20Json%20and%20Protobuf.xlsx) over Protobuf **only in cases which the binary content ratio is above 90%.** 
You should consider Jcob as an alternative if:
1. You need to serialize very large numeric arrays (multidimantional and jagged arrays are supported)
2. You can't (or don't want to) deal with ProtoMember attributes
3. You need to be able to read\edit manually the non-binary fields of the serialization result


