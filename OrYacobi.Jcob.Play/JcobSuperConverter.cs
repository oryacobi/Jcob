using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.IO;
using Accord.Math;
using Newtonsoft.Json;

namespace OrYacobi.Jcob.Play
{


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
    public string MiddleName { get; set; } = "Batman";

    [JsonConverter(typeof(JcobConverter))]
    public double[] ScoresArray { get; set; } = { 1.6, 18.03, 3, 9.8, 8.7, 4.989, 48.48, 2.045, 8.68, 343.656, 3.811, 77.2, 0.3091, 798, 0.57628, 6.2, 1.3, 54, 48.622, 70.52, 60.4628 };

    [JsonConverter(typeof(JcobAccordConverter))] //Mind the new converter type
    public ContinuousHistogram AccordHistogram { get; set; } = new ContinuousHistogram(new []{1,2,3}, new Range(0,10));
}
}
