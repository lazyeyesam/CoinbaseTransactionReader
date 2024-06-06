using System;
using System.Globalization;
using CoinbaseTransactionReader.Infrastructure.Interfaces;
using CoinbaseTransactionReader.Model;

namespace CoinbaseTransactionReader.Infrastructure.Parsers
{
    public abstract class TransactionParser : ITransactionParser
    {
        /// <summary>Parses the specified entry.</summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public Order Parse(string[] entry)
        {
            var transaction = CreateTransaction(entry);
            var order = new Order();
            ConvertTransaction(order, transaction);
            return order;

        }

        /// <summary>
        /// Creates the transaction.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        private CoinbaseTransaction CreateTransaction(string[] entry)
        {
            var transaction = new CoinbaseTransaction
            {
                Timestamp = DateTime.Parse(entry[0]),
                TransactionType = entry[1],
                Asset = entry[2],
                Quantity = decimal.Parse(entry[3], NumberStyles.Float),
                SpotPrice = decimal.Parse(entry[5], NumberStyles.Float),
                Notes = entry[9].Replace("\"", "").Replace("\\", "")
            };

            var subTotal = entry[6];
            if (!string.IsNullOrWhiteSpace(subTotal) && subTotal != "\"\"")
                transaction.SubTotal =  decimal.Parse(entry[5]);

            var total = entry[7];
            if (!string.IsNullOrWhiteSpace(total) && total != "\"\"")
                transaction.Total = decimal.Parse(total);

            var fees = entry[8];
            if (!string.IsNullOrWhiteSpace(fees) && fees != "\"\"")
                transaction.Fees = decimal.Parse(fees);

            return transaction;
        }

        /// <summary>
        /// Converts the transaction.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        protected virtual void ConvertTransaction(Order order, CoinbaseTransaction transaction)
        {
            order.Asset = transaction.Asset;
            order.Timestamp = transaction.Timestamp;
            order.Quantity = transaction.Quantity;
            order.Price = transaction.SpotPrice;
            order.SubTotal = transaction.SubTotal ?? 0;
            order.Fees = transaction.Fees ?? 0;
            order.Notes = transaction.Notes;
        }
    }
}

