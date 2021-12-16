using System;

using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<Serialization>();

            Console.WriteLine(summary); 
        }
    }
}
