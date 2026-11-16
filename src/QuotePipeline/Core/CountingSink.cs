using System.Diagnostics;

namespace QuotePipeline.Core;

/// <summary>
/// Day 1 用的最簡單接收端:只計數,並檢查序號有沒有跳號(gap)。
/// 假設只有單一生產者呼叫 Publish;讀取端(報表)用 Interlocked/Volatile 讀。
/// </summary>
public sealed class CountingSink : IQuoteSink
{
    private long _count;
    private long _gaps;
    private long _expectedSeq;
    private long _maxLatencyTicks;

    public long Count => Interlocked.Read(ref _count);
    public long Gaps => Interlocked.Read(ref _gaps);
    public double MaxLatencyMs => Interlocked.Read(ref _maxLatencyTicks) * 1000.0 / Stopwatch.Frequency;

    public void Publish(in Quote q)
    {
        if (q.Seq != _expectedSeq) Interlocked.Increment(ref _gaps);
        _expectedSeq = q.Seq + 1;

        long latency = Stopwatch.GetTimestamp() - q.Timestamp;
        if (latency > Volatile.Read(ref _maxLatencyTicks))
            Volatile.Write(ref _maxLatencyTicks, latency);

        Interlocked.Increment(ref _count);
    }
}
