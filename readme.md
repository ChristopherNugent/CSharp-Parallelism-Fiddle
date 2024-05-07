Just a little benchmark I made since there's all sorts of blog nonsense about Task.WhenAll and parallelism.

A rough summary of my findings is as follows:
    
    1. When an async function actually yields, it can be resumed by the threadpool and therefore will cause parallelism.
    2. Using a projection to Task.Run() will schedule all the work to be done on the ThreadPool, but this may be undesirable
    if the state of the tasks is heavy, and generally starves out other threads.
    3. Using Parallel.ForEachAsync() partitions tasks so that only a fixed amount are in progress at once. This prevents the
    mass of threads that may have been created by Task.Run() from causing issues. 