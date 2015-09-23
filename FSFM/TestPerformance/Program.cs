using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FSFMLibrary;
namespace TestPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            var func = FuzzyFactory.BuildFuzzyDesiredVelocity();
            Random r = new Random();
            var iterations = 1000000;
            var velocityTest = Enumerable.Range(0, iterations).Select(c => r.NextDouble() * 1.5d).ToArray();
            var soma = 0.0d;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for(var indice = 0; indice < iterations; indice++)
            {
                soma += func(velocityTest[indice]);
            }
            watch.Stop();

            Stopwatch watch2 = new Stopwatch();
            soma = 0.0d;
            watch2.Start();
            for (var indice = 0; indice < iterations; indice++)
            {
                soma += velocityTest[indice];
            }
            watch2.Stop();
            Console.WriteLine(watch.Elapsed.TotalMilliseconds - watch2.Elapsed.TotalMilliseconds);
            Console.ReadLine();
        }
    }
}
