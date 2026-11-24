using System.Diagnostics;

public class ThreadPoolExperiment
{
    
    public static void Run(bool useBlocking)
    {
        Console.WriteLine($"SetMinThreads 結果:{ThreadPool.SetMinThreads(200, 200)}");
        ThreadPool.GetMinThreads(out int minWorker, out int minIo);
        Console.WriteLine($"最小工人數:{minWorker}");    

        //ThreadPool.SetMaxThreads(200,200); // 限制起始工人數,讓現象明顯
        int orderCount = 500;
        var sw = Stopwatch.StartNew();
        var tasks = new Task[orderCount];

        //監視器：用『專屬執行緒,不佔用執行緒池
        var cts = new CancellationTokenSource();
        var monitor = new Thread(()=>
        {
            while(!cts.IsCancellationRequested)
            {
                Console.WriteLine($"  [執行緒池] 執行緒數 {ThreadPool.ThreadCount},排隊中 {ThreadPool.PendingWorkItemCount}");
                Thread.Sleep(500);
            }
        }){ IsBackground = true };

        monitor.Start();

        for(int i=0;i<orderCount;i++)
        {
           tasks[i] = Task.Run( async()=>
           {
               if(useBlocking)
               {
                  // ① 阻塞寫法:呼叫 SimulateIoAsync(),然後用 .Wait() 卡住這個工人等它完成
                    SimulateIoAsync().Wait();
               }
               else
               {
                   // ② 非同步寫法:用 await 等 SimulateIoAsync()
                   await SimulateIoAsync();
               }
           });
        }
        Task.WaitAll(tasks);
        sw.Stop();
        cts.Cancel();
        Console.WriteLine($"{(useBlocking ? "阻塞" : "await")}:{orderCount} 筆全部完成,耗時 {sw.ElapsedMilliseconds} ms\n");
    }
 // 模擬一次要等 1 秒的網路呼叫
    private static Task SimulateIoAsync() => Task.Delay(1000);

}