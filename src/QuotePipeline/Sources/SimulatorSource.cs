using System.Diagnostics;
using QuotePipeline.Core;

namespace QuotePipeline.Sources;

/// <summary>
/// 可控流量的行情模擬器。每個迴圈算出「到現在為止應該送出幾筆」,把落後的補送出去,
/// 所以速率由 ratePerSec 決定,而且會出現小突發(burst),比較像真實行情。
/// 注意:Windows 的 Thread.Sleep(1) 實際約 15ms,突發會比 Mac 大,這是正常的。
/// </summary>
public sealed class SimulatorSource : IQuoteSource
{
    private readonly int _ratePerSec;
    private readonly int _symbolCount;
    private readonly int _seed;

    public SimulatorSource(int ratePerSec, int symbolCount = 10, int seed = 42)
    {
        _ratePerSec = ratePerSec;
        _symbolCount = symbolCount;
        _seed = seed;
    }

    public Task RunAsync(IQuoteSink sink, CancellationToken ct)
    {
        // LongRunning:給它專用執行緒,不佔用 thread pool。
        return Task.Factory.StartNew(
            () => Loop(sink, ct),
            ct,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    private void Loop(IQuoteSink sink, CancellationToken ct)
    {
        var rng = new Random(_seed);
        var prices = new int[_symbolCount];
        for (int i = 0; i < prices.Length; i++) prices[i] = 10_000 + i * 100;

        var sw = Stopwatch.StartNew();
        long sent = 0;

        while (!ct.IsCancellationRequested)
        {
            long due = (long)(sw.Elapsed.TotalSeconds * _ratePerSec);

            while (sent < due && !ct.IsCancellationRequested)
            {
                int sym = rng.Next(_symbolCount);
                prices[sym] += rng.Next(-1, 2); // 價格隨機漫步 -1/0/+1
                var q = new Quote(sent, Stopwatch.GetTimestamp(), sym, prices[sym], rng.Next(1, 20));
                sink.Publish(in q);
                sent++;
            }

            Thread.Sleep(1);
        }
    }
}
