using BenchmarkDotNet.Running;

namespace Benchmark
{
    internal class Program
    {
        private static void Main()
        {
            _ = BenchmarkRunner.Run<AuthBenchmark>();
        }
    }
}
