using System.Diagnostics;

Console.WriteLine("行情模擬器啟動");
var sink = new ChannelSink();
//var sink = new PrintingSink();
SimulatorSource simulator = new SimulatorSource(1_000_000, 5, sink); 

// 消費者:故意很慢,每處理一筆睡 1 毫秒ß
var consumer = Task.Run(async () =>
{
    await foreach (var q in sink.Reader.ReadAllAsync())
    {
       Thread.Sleep(1); 
    }
});

// 監視器:每秒印出佇列積壓幾筆、記憶體用了多少
var monitor = Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(1000);
        Console.WriteLine($"佇列積壓 {sink.Reader.Count:N0} 筆,記憶體 {GC.GetTotalMemory(false) / 1024 / 1024} MB");
    }
});


simulator.Run();
Console.WriteLine($"生產結束,佇列還積壓 {sink.Reader.Count:N0} 筆");

//Console.WriteLine($"收到 {sink.Count:N0} 筆,速率 {sink.Count / sw.Elapsed.TotalSeconds:N0} 筆/秒");
//RaceExperiment.Run();
