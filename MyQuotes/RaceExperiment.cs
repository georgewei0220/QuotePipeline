using System.Diagnostics;

public class RaceExperiment
{
    public static void Run()
    {
       var counter = new Counter();
       var threads = new Thread[4];
       
        for(int i = 0; i < 4; i++)
        {
            threads[i] = new Thread(() =>
            {
                long local = 0; // ① 這個執行緒自己專屬的計數
                for (int j = 0; j < 1_000_000; j++)
                {
                    // ② 只對 local 加 1,不碰 counter
                    local++;
                }
                // ③ 迴圈結束後,把 local 一次加到 counter.Value 上
                Interlocked.Add(ref counter.Value, local);
            });
        }
        var total = Stopwatch.StartNew();      
        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();
        total.Stop();
        Console.WriteLine($"結果 {counter.Value:N0},預期 4,000,000,總耗時 {total.ElapsedMilliseconds} ms");
    }
}


public class Counter
{
    public long Value;
}