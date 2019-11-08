using Accord;
using Accord.Math;
using Newtonsoft.Json;

namespace OrYacobi.Jcob.Play.Models
{
    public class ExampleClass
    {
        public string FirstName { get; set; } = "James";

        public string MiddleName { get; set; } = "Batman";

        public string LastName { get; set; } = "Newton-King";

        public int Age { get; set; } = -1;

        [JsonConverter(typeof(JcobConverter))]
        public double[] ScoresArray { get; set; } = { 1.6, 18.03, 3, 9.8, 8.7, 4.989, 48.48, 2.045, 8.68, 343.656, 3.811, 77.2, 0.3091, 798, 0.57628, 6.2, 1.3, 54, 48.622, 70.52, 60.4628 };
    }
}
