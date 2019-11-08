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


}
