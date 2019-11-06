using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace OrYacobi.JsonWithBinaryTests
{
    [TestClass()]
    public class JsonWithBinaryConvertTests
    {
        [TestMethod()]
        public void SerializeObjectTest()
        {
            var classToSerielze = new TestJsonWithBinary {X = 5, Y = new byte[] {9, 9, 9}};
            var result = OrYacobi.Jcob.JcobConvert.SerializeObject(classToSerielze, Formatting.Indented, new JsonSerializerSettings());

            var deserializedClass = OrYacobi.Jcob.JcobConvert.DeserializeObject<TestJsonWithBinary>(result, new JsonSerializerSettings());
        }
        
        //TBD...

        public class TestJsonWithBinary
        {
            public int X { get; set; }
            public byte[] Y { get; set; }
        }
    }
}