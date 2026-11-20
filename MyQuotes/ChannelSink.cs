using System.Threading.Channels;

public class ChannelSink : IQuoteSink
{
    private readonly Channel<Quote> _channel = Channel.CreateUnbounded<Quote>();
    public ChannelReader<Quote> Reader => _channel.Reader;
    public void Publish(Quote q)
    {
        _channel.Writer.TryWrite(q);
    }
}