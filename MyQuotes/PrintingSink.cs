public class PrintingSink : IQuoteSink
{
    public void Publish(Quote quote)
    {
        Console.WriteLine($"發布行情: {quote.Seq}, 價格: {quote.Price}, 數量: {quote.Qty}");
    }
}