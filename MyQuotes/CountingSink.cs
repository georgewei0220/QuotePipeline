public class CountingSink : IQuoteSink
{
    public long Count { get; private set; } = 0;   
    public void Publish(Quote q)
    {
        Count++;
    }
}