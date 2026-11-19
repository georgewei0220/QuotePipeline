public class CountingSink
{
    public long Count { get; private set; } = 0;
    public void Publish(Quote q)
    {
        Count++;
    }
}