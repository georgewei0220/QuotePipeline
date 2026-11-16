using QuotePipeline.Core;
using QuotePipeline.Experiments;
using QuotePipeline.Sources;

if (args.Length == 0 || args[0] is "-h" or "--help")
{
    PrintHelp();
    return;
}

switch (args[0])
{
    case "race":
        RaceExperiment.Run();
        break;

    case "sim":
        await RunSimAsync(
            rate: GetInt(args, "--rate", 200_000),
            seconds: GetInt(args, "--seconds", 5));
        break;

    default:
        Console.WriteLine($"未知指令:{args[0]}");
        PrintHelp();
        break;
}

static async Task RunSimAsync(int rate, int seconds)
{
    Console.WriteLine($"模擬器:目標 {rate:N0} 筆/秒,跑 {seconds} 秒");

    var sink = new CountingSink();
    var source = new SimulatorSource(rate);
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));

    var producer = source.RunAsync(sink, cts.Token);

    long last = 0;
    for (int s = 1; s <= seconds; s++)
    {
        await Task.Delay(1000);
        long now = sink.Count;
        Console.WriteLine($"[{s,2}s] {now - last,10:N0} 筆/秒   累計 {now,12:N0}   跳號 {sink.Gaps}   最大延遲 {sink.MaxLatencyMs:F3} ms");
        last = now;
    }

    try { await producer; } catch (TaskCanceledException) { }

    Console.WriteLine();
    Console.WriteLine($"總計 {sink.Count:N0} 筆,跳號 {sink.Gaps}");
}

static int GetInt(string[] args, string name, int fallback)
{
    int i = Array.IndexOf(args, name);
    if (i >= 0 && i + 1 < args.Length && int.TryParse(args[i + 1], out int v)) return v;
    return fallback;
}

static void PrintHelp()
{
    Console.WriteLine("""
        用法:dotnet run -c Release -- <指令> [選項]

          race                            Day 2:競爭條件實驗(不保護 / lock / Interlocked)
          sim [--rate N] [--seconds S]    Day 1:模擬器 + 計數,預設 200000 筆/秒、5 秒
        """);
}
