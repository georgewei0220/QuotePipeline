using System.Diagnostics;
Console.WriteLine("行情模擬器啟動");

const int Rate = 200_000_000;   // 目標速率:每秒幾筆
const int Seconds = 2;      // 跑幾秒
long sent = 0;
var sink = new CountingSink();
var sw = Stopwatch.StartNew();

while (sw.Elapsed.TotalSeconds < Seconds)
{
    long due = (long)(sw.Elapsed.TotalSeconds * Rate);
    while (sent < due)
    {
        sink.Publish(new Quote(sent, 10000, 5));
        //Console.WriteLine($"已過 {sw.Elapsed.TotalSeconds:F3} 秒,應送累計 {due},現在送第 {sent} 筆");
        sent++;
    }
    Thread.Sleep(1);
}

sw.Stop();
double rate = sink.Count / sw.Elapsed.TotalSeconds;
Console.WriteLine($"目標 {Rate:N0} 筆/秒,實際收到 {sink.Count:N0} 筆,速率 {rate:N0} 筆/秒");

public readonly record struct Quote(long Seq, int Price, int Qty);

