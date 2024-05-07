using BenchmarkDotNet.Running;

namespace ParallelTest
{
    public class ParallelTest
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ParallelTaskBenchmark>();
        }
    }
}