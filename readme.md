## Summary

Just a little benchmark I made since there's all sorts of blog nonsense about Task.WhenAll and parallelism.

A rough summary of my findings is as follows:
    
1. When an async function actually yields, it can be resumed by the threadpool and therefore will cause parallelism.
    1. Inversely, if the thread does not yield (let's say your in-memory cache or mock returns without yielding anywhere), no parallelism is caused.
2. Using a projection to Task.Run() will schedule all the work to be done on the ThreadPool, but this may be undesirable if the state of the tasks is heavy, and generally starves out other threads.
3. Using Parallel.ForEachAsync() partitions tasks so that only a fixed amount are in progress at once. This prevents the mass of threads that may have been created by Task.Run() from causing issues. 

## Some Data

1. TaskCount is the number of tasks created.
2. WorkCount represents some amount of CPU bound work via a simple loop.
3. IoDelayMs is a delay between CPU bound work within the task, emulated using `Task.Delay(...)`. 


| Method                   | TaskCount | WorkCount | IoDelayMs | Mean        | Error     | StdDev    |
|------------------------- |---------- |---------- |---------- |------------:|----------:|----------:|
| ProjectTasksAndWhenAll   | 25        | 10000000  | 0         |   289.62 ms |  3.109 ms |  2.908 ms |
| ProjectTaskRunAndWhenAll | 25        | 10000000  | 0         |    57.54 ms |  1.150 ms |  1.791 ms |
| UseParallelForEachAsync  | 25        | 10000000  | 0         |    56.25 ms |  0.794 ms |  0.704 ms |
| ProjectTasksAndWhenAll   | 25        | 10000000  | 1         |    90.59 ms |  0.180 ms |  0.151 ms |
| ProjectTaskRunAndWhenAll | 25        | 10000000  | 1         |    52.30 ms |  0.565 ms |  0.528 ms |
| UseParallelForEachAsync  | 25        | 10000000  | 1         |    65.52 ms |  0.611 ms |  0.572 ms |
| ProjectTasksAndWhenAll   | 25        | 50000000  | 0         | 1,443.16 ms |  5.492 ms |  5.137 ms |
| ProjectTaskRunAndWhenAll | 25        | 50000000  | 0         |   267.03 ms |  5.256 ms |  8.781 ms |
| UseParallelForEachAsync  | 25        | 50000000  | 0         |   295.70 ms |  5.625 ms |  7.114 ms |
| ProjectTasksAndWhenAll   | 25        | 50000000  | 1         |   450.27 ms |  7.586 ms |  9.594 ms |
| ProjectTaskRunAndWhenAll | 25        | 50000000  | 1         |   272.23 ms |  5.363 ms |  6.782 ms |
| UseParallelForEachAsync  | 25        | 50000000  | 1         |   301.33 ms |  5.841 ms |  5.464 ms |
| ProjectTasksAndWhenAll   | 50        | 10000000  | 0         |   588.07 ms | 10.582 ms |  9.381 ms |
| ProjectTaskRunAndWhenAll | 50        | 10000000  | 0         |   107.31 ms |  1.722 ms |  1.843 ms |
| UseParallelForEachAsync  | 50        | 10000000  | 0         |   106.08 ms |  1.890 ms |  1.675 ms |
| ProjectTasksAndWhenAll   | 50        | 10000000  | 1         |   171.52 ms |  2.183 ms |  2.042 ms |
| ProjectTaskRunAndWhenAll | 50        | 10000000  | 1         |   105.20 ms |  2.054 ms |  2.365 ms |
| UseParallelForEachAsync  | 50        | 10000000  | 1         |   123.23 ms |  1.930 ms |  1.806 ms |
| ProjectTasksAndWhenAll   | 50        | 50000000  | 0         | 2,896.39 ms | 15.657 ms | 14.645 ms |
| ProjectTaskRunAndWhenAll | 50        | 50000000  | 0         |   520.02 ms | 10.274 ms | 12.230 ms |
| UseParallelForEachAsync  | 50        | 50000000  | 0         |   528.44 ms | 10.365 ms |  9.188 ms |
| ProjectTasksAndWhenAll   | 50        | 50000000  | 1         |   835.96 ms |  5.749 ms |  5.096 ms |
| ProjectTaskRunAndWhenAll | 50        | 50000000  | 1         |   512.02 ms | 10.151 ms | 14.230 ms |
| UseParallelForEachAsync  | 50        | 50000000  | 1         |   543.74 ms |  8.644 ms |  7.663 ms |