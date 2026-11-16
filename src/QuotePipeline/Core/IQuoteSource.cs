namespace QuotePipeline.Core;

/// <summary>接收行情的一端。之後會有:CountingSink、ChannelSink、SocketSink...</summary>
public interface IQuoteSink
{
    void Publish(in Quote quote);
}

/// <summary>
/// 行情來源。模擬器、重播器(讀 Quotes.db)、元大 Live(只能在 Windows)都實作這個介面,
/// 管線其他部分不用知道資料從哪來。
/// </summary>
public interface IQuoteSource
{
    Task RunAsync(IQuoteSink sink, CancellationToken ct);
}
