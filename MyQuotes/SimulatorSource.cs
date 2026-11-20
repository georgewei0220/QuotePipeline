using System.Diagnostics;

public class SimulatorSource
{
    private readonly int _rate;
    private readonly int _seconds;
    private readonly IQuoteSink _sink;

    public SimulatorSource(int rate, int seconds, IQuoteSink sink)
    {
        _rate = rate;
        _seconds = seconds;
        _sink = sink;
    }

    public void Run()
    {
        long sent = 0;
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed.TotalSeconds < _seconds)
        {
            long due = (long)(sw.Elapsed.TotalSeconds * _rate);
            while (sent < due)
            {
                _sink.Publish(new Quote(sent, 10000, 5));
                sent++;
            }
            Thread.Sleep(1);
        }
        sw.Stop();
        //double rate = _sink.Count / sw.Elapsed.TotalSeconds;
        //Console.WriteLine($"目標 {_rate:N0} 筆/秒,實際收到 {_sink.Count:N0} 筆,速率 {rate:N0} 筆/秒");
    }    

}