using Accord;
using Accord.Math;
using Newtonsoft.Json;
using OrYacobi.Jcob.Play.Business;

namespace OrYacobi.Jcob.Play.Models
{
    public class ExampleAccord
    {
        public string MiddleName { get; set; } = "Batman";

        [JsonConverter(typeof(JcobConverter))]
        public double[] ScoresArray { get; set; } = { 1.6, 18.03, 3, 9.8, 8.7, 4.989, 48.48, 2.045, 8.68, 343.656, 3.811, 77.2, 0.3091, 798, 0.57628, 6.2, 1.3, 54, 48.622, 70.52, 60.4628 };

        [JsonConverter(typeof(JcobAccordConverter))] //Mind the new converter type
        public ContinuousHistogram AccordHistogram { get; set; } = new ContinuousHistogram(new[] { 1, 2, 3 }, new Range(0, 10));
    }
}
