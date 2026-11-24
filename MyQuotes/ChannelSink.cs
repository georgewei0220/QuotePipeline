using System.Threading.Channels;

public class ChannelSink : IQuoteSink
{
    //private readonly Channel<Quote> _channel = Channel.CreateUnbounded<Quote>();
    private readonly Channel<Quote> _channel;
    public ChannelSink(int capacity)
    {
        var option = new BoundedChannelOptions(capacity)
        {
           FullMode = BoundedChannelFullMode.DropOldest
        };
        _channel = Channel.CreateBounded<Quote>(option);
    }
    public long Written { get; private set; }
    public ChannelReader<Quote> Reader => _channel.Reader;
    public void Publish(Quote q)
    {
        while(!_channel.Writer.TryWrite(q))
        {
            Thread.Yield();
        }
        Written++;
    }

}