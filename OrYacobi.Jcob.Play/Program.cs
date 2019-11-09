using System;
using OrYacobi.Jcob.Play.Business;

namespace OrYacobi.Jcob.Play
{
    class Program
    {
        static void Main()
        {
            PerformanceTests test = new PerformanceTests();
            var performanceResults = test.Run();

            foreach (PerformanceTests.PerformanceResults result in performanceResults)
            {
                Console.WriteLine(result);
            }

            Console.ReadLine();
        }
    }
}
