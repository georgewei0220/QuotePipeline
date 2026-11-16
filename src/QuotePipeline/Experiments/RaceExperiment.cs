using System.Diagnostics;

namespace QuotePipeline.Experiments;

/// <summary>
/// Day 2 實驗:多執行緒對同一個計數器累加,比較「不保護 / lock / Interlocked」。
/// 重點不是背答案,而是親眼看到:(1) 不保護會少算 (2) 兩種修法的速度差多少。
/// 一定要用 Release 跑:dotnet run -c Release -- race
/// </summary>
public static class RaceExperiment
{
    private const int Threads = 4;
    private const int PerThread = 1_000_000;

    private sealed class Counter
    {
        public long Value;
    }

    public static void Run()
    {
        Console.WriteLine($"{Threads} 個執行緒,每個累加 {PerThread:N0} 次,預期結果 {Threads * (long)PerThread:N0}");
        Console.WriteLine();

        Measure("不保護 (c.Value++)", c =>
        {
            for (int i = 0; i < PerThread; i++) c.Value++;
        });

        Measure("lock", c =>
        {
            for (int i = 0; i < PerThread; i++)
            {
                lock (c) { c.Value++; }
            }
        });

        Measure("Interlocked.Increment", c =>
        {
            for (int i = 0; i < PerThread; i++) Interlocked.Increment(ref c.Value);
        });

        Console.WriteLine();
        Console.WriteLine("思考題:");
        Console.WriteLine(" 1. 「不保護」為什麼會少算?c.Value++ 在 CPU 層級是哪幾個步驟?");
        Console.WriteLine(" 2. lock 和 Interlocked 的耗時差多少?為什麼?");
        Console.WriteLine(" 3. 把 Threads 改成 1、2、8,結果和耗時怎麼變?");
    }

    private static void Measure(string name, Action<Counter> work)
    {
        var counter = new Counter();
        var threads = new Thread[Threads];
        for (int i = 0; i < Threads; i++)
            threads[i] = new Thread(() => work(counter));

        var sw = Stopwatch.StartNew();
        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();
        sw.Stop();

        long expected = Threads * (long)PerThread;
        long lost = expected - counter.Value;
        Console.WriteLine($"{name,-26} 結果 {counter.Value,12:N0}  少算 {lost,10:N0}  耗時 {sw.ElapsedMilliseconds,5} ms");
    }
}
