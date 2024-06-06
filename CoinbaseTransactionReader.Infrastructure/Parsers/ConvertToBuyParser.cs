using System.Globalization;
using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure.Parsers
{
    public class ConvertToBuyParser : TransactionParser
    {
        protected override void ConvertTransaction(Order order, CoinbaseTransaction transaction)
        {
            base.ConvertTransaction(order, transaction);
            order.OrderType = OrderType.Buy;
            order.Fees = 0;
            order.Quantity = GetQuantity(transaction);
            order.Asset = GetAsset(transaction);

            if (transaction.Total != null)
            {
                order.SubTotal = transaction.Total.Value;
                order.Price = transaction.Total.Value / order.Quantity;
            }
        }

        private string GetAsset(CoinbaseTransaction transaction)
        {
            var parts = transaction.Notes.Split(' ');
            var asset = parts[5];
            return asset;
        }

        private decimal GetQuantity(CoinbaseTransaction transaction)
        {
            var parts = transaction.Notes.Split(' ');
            var quantity = decimal.Parse(parts[4].Replace(",",""), NumberStyles.Float);
            return quantity;

        }
    }
}