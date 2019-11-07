# Jcob
Jcob is an extension to Json.NET which enables to combine a binary content in a Json format (efficiently!)

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

When to use it?
---------------
1. You need to serialize large numeric arrays 
2. You need to combine a binary serializer result (or any other binary content) in your Json 
3. You like to work with the Json structure
4. You tested other aproches and disapointed from the results and performance

When **NOT** to use is?
-----------------------
1. You are **striced to a valid Json** format 
2. You need to be able to see and understand **all** the contant in the serialization result
3. The total amount of binary contant is very samall (in this case, consider [this](https://stackoverflow.com/questions/1443158/binary-data-in-json-string-something-better-than-base64))

Dependencies
------------
.NET Standard 2.0 ([Full dependency table](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support))

License
-------
[MIT](../master/LICENSE)

Usage Examples
--------------
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

If you want to serialize it to a normal Json, simply call JsonConvert instead: (and the Jcob converter will disable itself)
```csharp
string jsonString = JsonConvert.SerializeObject(originalObject);
```

You can extand the converter usage to additional object types. In the following example I needed to combine an Accord.Math histogram in my Jcob. 
As long as you know how to **convert your object into bytes, and back from bytes to object** - it's pretty simple
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
