using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure.Parsers
{
    public class ConvertToSellParser : TransactionParser
    {
        protected override void ConvertTransaction(Order order, CoinbaseTransaction transaction)
        {
            base.ConvertTransaction(order, transaction);
            order.OrderType = OrderType.Sell;
            order.Fees = 0;

            if (transaction.Total != null)
                order.SubTotal = transaction.Total.Value;

        }
    }
}