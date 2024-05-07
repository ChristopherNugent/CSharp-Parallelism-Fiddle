using BenchmarkDotNet.Attributes;

namespace ParallelTest;

public class ParallelTaskBenchmark
{

    [Params( 10, 50)]
    public int TaskCount { get; set; }

    [Params( 1_000_000, 50_000_000) ]
    public int WorkCount { get; set; }

    [Params( 1, 100 )]
    public int IoDelayMs { get; set; } 


    [Benchmark]
    public async Task ProjectTasksAndWhenAll()
    {
        var tasks = Enumerable.Range(0, TaskCount).Select(_ => DoMixedWork());
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task ProjectTaskRunAndWhenAll()
    {
        var tasks = Enumerable.Range(0, TaskCount).Select(_ => Task.Run(() => DoMixedWork()));
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task UseParallelForEachAsync()
    {
        await Parallel.ForEachAsync(
            Enumerable.Range(0, TaskCount),
            async (_, _) => await DoMixedWork());
    }

    private async Task DoMixedWork()
    {
        CpuBoundWork();
        await IoBoundWork();
        CpuBoundWork();
        await IoBoundWork();
        CpuBoundWork();
        await IoBoundWork();
        CpuBoundWork();
    }

    /// <summary>
    /// Emulates an IO operation. If IoDelayMs is zero, executes synchronously.
    /// </summary>
    /// <returns></returns>
    private async Task IoBoundWork()
    {
        await Task.Delay(IoDelayMs);
    }

    /// <summary>
    /// Emulates CPU bound work.
    /// </summary>
    /// <returns></returns>
    private int CpuBoundWork()
    {
        var sum = 0;
        for (int i = 0; i < WorkCount; i++)
        {
            sum += i;
        }

        return sum;
    }
}
