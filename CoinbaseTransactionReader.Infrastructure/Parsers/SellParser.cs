using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure.Parsers
{
    public class SellParser : TransactionParser
    {
        protected override void ConvertTransaction(Order order, CoinbaseTransaction transaction)
        {
            base.ConvertTransaction(order, transaction);
            order.OrderType = OrderType.Sell;
        }
    }
}
